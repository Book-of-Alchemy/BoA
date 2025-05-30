using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStats), typeof(CharacterAnimator), typeof(SpriteRenderer))]
public class DungeonBehavior : PlayerBaseBehavior
{
    [Header("Dungeon Settings")]
    [SerializeField] private float _inputBufferDuration = 0.1f;
    public GameObject highlightPrefab;

    // Components & 상태
    private PlayerStats _stats;
    private CharacterAnimator _animator;
    private SpriteRenderer _spriteRenderer;


    public bool canMove = true;

    private bool _isMoving;
    private bool _isCtrlHeld;
    private bool _isDashHeld;

    private float _startHp;
    private HashSet<CharacterStats> _initialEnemiesInSight;

    // 코루틴 핸들
    private Coroutine _moveBuffer;
    private Coroutine _attackBuffer;
    private Coroutine _highlightBuffer;
    private Coroutine _mousePathCoroutine;
    // 반복 이동용 코루틴 핸들
    private Coroutine _holdMoveCoroutine;

    private bool _isMouseMove = false;
    private List<Tile> _mouseMovePath;    // 남은 경로 저장용

    // 마지막 이동 방향 (하이라이트용)
    private Vector2Int _lastMoveDir = Vector2Int.right;

    // 달리기(Shift) 상태 저장
    private float _savedTurnSpeed;

    // 하이라이트 인스턴스
    private GameObject _highlightInstance;

    // 컨트롤 키로 선택된 방향 저장용
    private Vector2Int _ctrlSelectedDir;

    // 아이템 사용
    private BaseItem _currentItem;
    [SerializeField] private bool _isItemUsing = false;

    // 마우스 경로 이동 시 시간 조절용 필드
    //private float _savedMouseTimeScale;
    //private float _savedMouseTurnSpeed;

    //UI가 열려있는지 체크
    private bool IsUIOpen()
    {
        // 필요한 UI 창 타입을 모두 체크
        return UIManager.IsOpened<UI_Menu>()
            || UIManager.IsOpened<UI_Inventory>()
            || UIManager.IsOpened<UI_DungeonResult>()
            || UIManager.IsOpened<UI_Equipment>()
            || UIManager.IsOpened<UI_LvSelect>()
            || UIManager.IsOpened<UI_Setting>();
    }

    public event Action OnPlayerMoved;

    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);

        _stats = GetComponent<PlayerStats>();
        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        InputManager.Instance.EnableMouseTracking = true;

        if (highlightPrefab != null)
        {
            _highlightInstance = Instantiate(highlightPrefab);
            _highlightInstance.SetActive(false);
        }
        else
        {
        }


        SubscribeInput();
    }

    protected override void SubscribeInput()
    {
        InputManager.OnMove += HandleMove;
        InputManager.OnMove += HandleHoldMove;
        InputManager.OnAttack += HandleAttack;
        InputManager.OnDashStart += HandleDashStart;   // Shift 누름
        InputManager.OnDashEnd += HandleDashEnd;     // Shift 뗌
        InputManager.OnCtrlStart += HandleCtrlStart;
        InputManager.OnCtrlEnd += HandleCtrlEnd;
        InputManager.OnInteract += HandleInteract;
        InputManager.Instance.OnCancel += HandleCancel;
        InputManager.Instance.OnMenu += HandleMenu;
        InputManager.OnRest += HandleRest;

        InputManager.OnMouseMove += HandleMouseMove;
        InputManager.OnMouseClick += HandleMouseClick;
    }

    public void SSubscribeInput()//외부 호출용
    {
        SubscribeInput();
    }
    protected override void UnsubscribeInput()
    {
        InputManager.OnMove -= HandleMove;
        InputManager.OnMove -= HandleHoldMove;
        InputManager.OnAttack -= HandleAttack;
        InputManager.OnDashStart -= HandleDashStart;
        InputManager.OnDashEnd -= HandleDashEnd;
        InputManager.OnCtrlStart -= HandleCtrlStart;
        InputManager.OnCtrlEnd -= HandleCtrlEnd;
        InputManager.OnInteract -= HandleInteract;
        InputManager.Instance.OnCancel -= HandleCancel;
        InputManager.Instance.OnMenu -= HandleMenu;
        InputManager.OnRest -= HandleRest;

        InputManager.OnMouseMove -= HandleMouseMove;
        InputManager.OnMouseClick -= HandleMouseClick;
    }
    public void UnsubscribeConInput()
    {
        InputManager.OnMove -= HandleMove;
        InputManager.OnMove -= HandleHoldMove;
        InputManager.OnAttack -= HandleAttack;
        InputManager.OnDashStart -= HandleDashStart;
        InputManager.OnDashEnd -= HandleDashEnd;
        InputManager.OnCtrlStart -= HandleCtrlStart;
        InputManager.OnCtrlEnd -= HandleCtrlEnd;
        InputManager.OnInteract -= HandleInteract;
        InputManager.OnRest -= HandleRest;
        InputManager.Instance.OnMenu -= HandleMenu;
        InputManager.OnMouseMove -= HandleMouseMove;
        InputManager.OnMouseClick -= HandleMouseClick;
    }

    void Shake()
    {
        transform.DOShakePosition(
            duration: 0.2f,
            strength: new Vector3(0.2f, 0, 0),  // 좌우 흔들림 세기 (X축만)
            vibrato: 30,            // 진동 횟수
            randomness: 0,
            snapping: false,
            fadeOut: true           // 점점 작아지게
        );
    }


    // ────────────────────────────────────────────────────────
    //  마우스 이동(하이라이트) 처리
    private void HandleMouseMove(Vector3 worldPos)
    {
        if (IsUIOpen() || _currentItem != null)
        {
            _highlightInstance.SetActive(false);
            return;
        }

        //아이템 사용중일땐 하이라이트타일 안뜸
        if (_currentItem != null)
        {
            _highlightInstance.SetActive(false);
            return;
        }

        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.y)
        );


        if (_stats.curLevel != null && !_stats.curLevel.tiles.ContainsKey(gridPos))
        {
            _highlightInstance.SetActive(false);
            return;
        }

        _highlightInstance.SetActive(true);
        _highlightInstance.transform.position = new Vector3(gridPos.x, gridPos.y, 0);
        var sr = _highlightInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            int y = gridPos.y;
            sr.sortingOrder = y == 0 ? -1 : (y > 0 ? -y * 11 : -y * 9);
        }
    }

    // ────────────────────────────────────────────────────────
    //  마우스 클릭으로 목적지 지정 후 경로 이동
    private void HandleMouseClick(Vector3 worldPos)
    {
        //UI열려있으면 무시
        if (IsUIOpen() || _currentItem != null)
            return;

        //아이템 사용중일 땐 이동 무시
        if (_currentItem != null)
        {
            _highlightInstance.SetActive(false);
            return;
        }

        Vector2Int goalPos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.y)
        );

        if (!_stats.curLevel.tiles.TryGetValue(goalPos, out Tile goalTile))
            return;

        if (_isMouseMove)
        {
            StopMousePathMovement();
            StartNewMousePath(goalPos, goalTile);
            return;
        }

        if (!Controller.isPlayerTurn || _isMoving)
            return;

        // 공격 조건
        Vector2Int curPos = _stats.CurTile.gridPosition;
        Vector2Int delta = goalPos - curPos;
        bool isAdjacent = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y)) == 1;

        if (goalTile.CharacterStatsOnTile != null && isAdjacent)
        {
            var targetStats = goalTile.CharacterStatsOnTile;

            if (targetStats.gameObject.CompareTag("NPC"))
            {
                HandleInteract();
                return;
            }
            else
            {
                ExecuteMouseAttack(goalTile);
                return;
            }
        }

        // 이동 처리
        StartNewMousePath(goalPos, goalTile);
    }

    private void StartNewMousePath(Vector2Int goalPos, Tile goalTile)
    {
        // 턴 상태 확인 추가
        if (!Controller.isPlayerTurn || _isMoving)
            return;

        // 이동 처리
        List<Tile> path = AstarPlayerPathFinder.FindPath(_stats.CurTile, goalTile, _stats.curLevel);
        if (path == null || path.Count <= 1)
            return;

        // 남은 이동 경로를 멤버 변수에 저장
        _mouseMovePath = path;
        _isMouseMove = true;

        // 시간 스케일 5배 적용
        //_savedMouseTimeScale = Time.timeScale;
        //_savedMouseTurnSpeed = TurnManager.Instance.turnSpeed;
        //Time.timeScale = _savedMouseTimeScale * 5f;
        //TurnManager.Instance.turnSpeed = _savedMouseTurnSpeed * 5f;

        // 이동 중단 검사 초기화
        _startHp = _stats.CurrentHealth;
        _initialEnemiesInSight = new HashSet<CharacterStats>(GetEnemiesInSight());

        // 코루틴 실행
        if (_mousePathCoroutine != null)
            StopMousePathMovement();
        _mousePathCoroutine = StartCoroutine(MoveAlongPath());
    }

    private void ExecuteMouseAttack(Tile targetTile)
    {
        //UI열려있으면 무시
        if (IsUIOpen() || _currentItem != null || !Controller.isPlayerTurn || _isMoving || _mousePathCoroutine != null)
            return;
        // 회전 및 애니메이터 설정 (기존 공격 방향 사용)
        Vector2Int dir = targetTile.gridPosition - _stats.CurTile.gridPosition;
        _lastMoveDir = dir;
        if (dir.x != 0)
            _spriteRenderer.flipX = dir.x < 0;

        // 공격 애니메이션 재생
        _animator.PlayAttack();
    }
    private IEnumerator MoveAlongPath()
    {
        // 현재 위치 제외
        _mouseMovePath.RemoveAt(0);

        foreach (var tile in _mouseMovePath)
        {
            //if (!Controller.isPlayerTurn)
            //    break;

            // 이전 이동이 끝날 때까지 대기
            yield return new WaitUntil(() => !_isMoving);

            if (tile.CharacterStatsOnTile != null)
            {
                StopMousePathMovement();
                yield break;
            }

            // 중단 조건 검사
            if (_stats.CurrentHealth < _startHp ||
                HasNewEnemy(_initialEnemiesInSight, GetEnemiesInSight()))
            {
                break;
            }

            // 한 칸 이동 실행
            Vector2Int dir = tile.gridPosition - _stats.CurTile.gridPosition;
            ExecuteMove(dir);

            // 이동 애니메이션이 완전히 끝날 때까지 대기
            yield return new WaitUntil(() => !_isMoving);

            // 턴 매니저가 플레이어 턴을 다시 열어줄 때까지 대기
            yield return new WaitUntil(() => Controller.isPlayerTurn);

            
        }
        //TurnManager.Instance.turnSpeed = _savedMouseTurnSpeed;
        //Time.timeScale = _savedMouseTimeScale;

        // 자동이동 종료
        _isMouseMove = false;
        _mouseMovePath = null;
        _mousePathCoroutine = null;
    }
    // ────────────────────────────────────────────────────────
    //  공통 버퍼 입력 코루틴
    private IEnumerator BufferInput(Action<Vector2Int> onBuffered)
    {
        float elapsed = 0f;
        Vector2 rawBuf = InputManager.MoveInput;

        while (elapsed < _inputBufferDuration)
        {
            // 턴 상태가 변경되면 즉시 중단
            if (!Controller.isPlayerTurn)
            {
                yield break;
            }

            Vector2 raw = InputManager.MoveInput;
            if (raw.x != 0) rawBuf.x = raw.x;
            if (raw.y != 0) rawBuf.y = raw.y;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 버퍼링 완료 후 다시 한 번 턴 상태 확인
        if (!Controller.isPlayerTurn)
        {
            yield break;
        }

        var dir = new Vector2Int(
            rawBuf.x > 0 ? 1 : rawBuf.x < 0 ? -1 : 0,
            rawBuf.y > 0 ? 1 : rawBuf.y < 0 ? -1 : 0
        );

        if (dir != Vector2Int.zero)
            onBuffered(dir);
    }

    // ────────────────────────────────────────────────────────
    //  이동 처리
    private void HandleMove(Vector2 raw)
    {
        //UI열려있으면 무시
        if (IsUIOpen())
            return;

        if (raw == Vector2.zero)
            return;

        // 자동이동 중이면 중단하고 수동 이동 실행
        if (_isMouseMove)
        {
            StopMousePathMovement();
            // 자동이동 중단 후 즉시 이동 실행
            if (Controller.isPlayerTurn && _moveBuffer == null)
            {
                _moveBuffer = StartCoroutine(BufferInput(dir =>
                {
                    ExecuteMove(dir);
                    _moveBuffer = null;
                }));
            }
            return;
        }

        if (!Controller.isPlayerTurn ||
            _isMoving)
            return;

        _moveBuffer = StartCoroutine(BufferInput(dir =>
        {
            ExecuteMove(dir);
            _moveBuffer = null;
        }));
    }

    private void ExecuteMove(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return;

        // 실행 직전에 다시 한 번 턴 상태 확인
        if (!Controller.isPlayerTurn || _isMoving)
            return;

        _lastMoveDir = dir;

        if (dir.x != 0)
            _spriteRenderer.flipX = dir.x < 0;

        if (_isCtrlHeld) return;

        var cur = _stats.CurTile.gridPosition;
        var nxt = cur + dir;

        if (!_stats.curLevel.tiles.TryGetValue(nxt, out var tile) ||
            !tile.IsWalkable ||
             tile.CharacterStatsOnTile != null)
        {
            Shake();
            return;
        }

        _isMoving = true;
        //_stats.CurTile.CharacterStatsOnTile = null;
        //_stats.CurTile = tile;
        //tile.CharacterStatsOnTile = _stats;

        _spriteRenderer.sortingOrder = -nxt.y * 10 + 1;
        _animator.PlayMove();

        transform.DOKill();

        // 피격 또는 신규 적 발견 시 즉시 중단
        if (_stats.CurrentHealth < _startHp ||
            HasNewEnemy(_initialEnemiesInSight, GetEnemiesInSight()))
        {
            StopMousePathMovement();
        }

        transform
            .DOMove(new Vector3(nxt.x, nxt.y, 0), 1 / (Controller.moveSpeed))
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 애니메이션이 완전히 끝난 시점에 이동 플래그 해제 및 턴 소비
                _isMoving = false;
                _stats.MoveToTile(tile);
                Controller.onActionConfirmed?.Invoke();
                CameraController.Instance.RestoreCameraState();
            });
    }

    // ────────────────────────────────────────────────────────
    //  공격 처리
    private void HandleAttack()
    {
        //UI열려있으면 무시
        if (IsUIOpen())
            return;


        if (_isMouseMove)
        {
            StopMousePathMovement();
            Vector2Int targetTransform = _stats.CurTile.gridPosition + _lastMoveDir;
            if (_stats.curLevel.tiles.TryGetValue(targetTransform, out Tile targetTile) &&
                targetTile.CharacterStatsOnTile != null &&
                targetTile.CharacterStatsOnTile.gameObject.CompareTag("NPC"))
            {
                return;
            }
            if (Controller.isPlayerTurn && _attackBuffer == null)
            {
                _attackBuffer = StartCoroutine(BufferAttack());
            }
            return;
        }

        if (!Controller.isPlayerTurn || _attackBuffer != null)
            return;
        Vector2Int targetPos = _stats.CurTile.gridPosition + _lastMoveDir;
        if (_stats.curLevel.tiles.TryGetValue(targetPos, out Tile frontTile) &&
            frontTile.CharacterStatsOnTile != null &&
            frontTile.CharacterStatsOnTile.gameObject.CompareTag("NPC"))
        {
            return;
        }

        _attackBuffer = StartCoroutine(BufferAttack());
    }

    private IEnumerator BufferAttack()
    {
        yield return new WaitForSeconds(_inputBufferDuration);

        if (!Controller.isPlayerTurn)
        {
            _attackBuffer = null;
            yield break;
        }

        _animator.PlayAttack();

        _attackBuffer = null;
    }
    public void OnAttackHit()
    {
        if (!Controller.isPlayerTurn)
            return;

        Vector2Int targetPos = _stats.CurTile.gridPosition + _lastMoveDir;
        if (_stats.curLevel.tiles.TryGetValue(targetPos, out Tile tile)
            && tile.CharacterStatsOnTile != null)
        {
            _stats.Attack(tile.CharacterStatsOnTile);
            CameraController.Instance.DoImpulse();
        }
        Controller.onActionConfirmed?.Invoke();
    }
    // ────────────────────────────────────────────────────────
    //  Shift 누름 시: 타임스케일 증가
    private void HandleDashStart()
    {
        //UI열려있으면 무시
        if (IsUIOpen())
            return;

        if (_isDashHeld) return;
        _isDashHeld = true;

        var tm = TurnManager.Instance;
        _savedTurnSpeed = tm.turnSpeed;
        tm.turnSpeed *= 10;
        Controller.moveSpeed *= 10;
        //Time.timeScale *= 10;
    }

    // ────────────────────────────────────────────────────────
    // Shift 해제 시: 원상 복구
    private void HandleDashEnd()
    {
        //UI열려있으면 무시
        if (IsUIOpen())
            return;

        if (!_isDashHeld) return;
        _isDashHeld = false;

        var tm = TurnManager.Instance;
        tm.turnSpeed = _savedTurnSpeed;
        Controller.moveSpeed /= 10;
        //Time.timeScale /= 10;
    }

    // ────────────────────────────────────────────────────────
    // Ctrl + 하이라이트
    private void HandleCtrlStart()
    {
        //UI열려있으면 무시
        if (IsUIOpen())
            return;

        _isCtrlHeld = true;
        _ctrlSelectedDir = _lastMoveDir; // 현재 방향으로 초기화

        if (_highlightInstance == null)
            _highlightInstance = Instantiate(highlightPrefab);

        _highlightInstance.SetActive(true);
        UpdateHighlightPosition();
        _highlightBuffer = StartCoroutine(HighlightWithBuffer());
    }

    private void HandleCtrlEnd()
    {
        //UI열려있으면 무시
        if (IsUIOpen())
            return;

        _isCtrlHeld = false;
        if (_highlightBuffer != null)
            StopCoroutine(_highlightBuffer);

        // 컨트롤을 뗄 때 선택된 방향을 _lastMoveDir에 저장
        if (_ctrlSelectedDir != Vector2Int.zero)
        {
            _lastMoveDir = _ctrlSelectedDir;
        }

        HideHighlight();
    }

    private IEnumerator HighlightWithBuffer()
    {
        while (_isCtrlHeld)
        {
            yield return BufferInput(dir =>
            {
                _ctrlSelectedDir = dir;
                UpdateHighlightPosition();
            });
        }
        HideHighlight();
    }

    private void UpdateHighlightPosition()
    {
        var cur = _stats.CurTile.gridPosition;
        var tgt = cur + _ctrlSelectedDir;

        if (!_stats.curLevel.tiles.TryGetValue(tgt, out var tile))
        {
            _highlightInstance.SetActive(false);
            return;
        }

        _highlightInstance.SetActive(true);
        _highlightInstance.transform.position = new Vector3(tgt.x, tgt.y, 0);

        var sr = _highlightInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            int y = tgt.y;
            sr.sortingOrder = y == 0
                ? -1
                : (y > 0 ? -y * 11 : -y * 9);
        }
    }

    private void HideHighlight()
    {
        if (_highlightInstance != null)
            _highlightInstance.SetActive(false);
    }

    // ────────────────────────────────────────────────────────
    // 상호작용 & 아이템 사용
    private void HandleInteract()
    {
        //UI열려있으면 무시
        if (IsUIOpen())
            return;

        if (_isMouseMove)
        {
            StopMousePathMovement();
        }

        if (!Controller.isPlayerTurn)
            return;

        // 플레이어가 바라보는 방향의 타일 위치
        Vector2Int frontPos = _stats.CurTile.gridPosition + _lastMoveDir;

        if (_stats.curLevel.tiles.TryGetValue(frontPos, out Tile frontTile) &&
            frontTile.CharacterStatsOnTile != null)
        {
            var targetStats = frontTile.CharacterStatsOnTile;

            if (targetStats.gameObject.CompareTag("NPC"))
            {
                return;
            }
        }
    }

    public void UseItem(ItemData data)
    {
        if (!Controller.isPlayerTurn) return;

        if (_isItemUsing) return;

        if (_isMouseMove)
        {
            StopMousePathMovement();
        }

        UnsubscribeConInput();
        _currentItem = ItemFactory.Instance.CreateItem(data.id);

        if (_currentItem == null) return;

        _highlightInstance.SetActive(false);
        _currentItem.ItemUseDone += HandleItemUseDone;
        _currentItem.UseItem(data);
    }

    private void HandleItemUseDone()
    {
        if (_currentItem == null) return;

        _isItemUsing = false;
        _currentItem.ItemUseDone -= HandleItemUseDone;

        // 아이템 사용 완료 시에도 턴 상태 확인
        if (Controller.isPlayerTurn)
        {
            Controller.onActionConfirmed?.Invoke();
        }

        _currentItem = null;
        InputManager.Instance.EnableMouseTracking = true;
        SubscribeInput();
    }
    // ──────────── 취소 키(X / ESC) ────────────
    private void HandleCancel()
    {
        // 아이템 사용 중이면 취소
        if (_currentItem != null)
        {
            InputManager.Instance.EnableMouseTracking = true;
            _highlightInstance.SetActive(true);

            _currentItem.CancelUse();
            _currentItem = null;
            _isItemUsing = false;
            //SubscribeInput();
            return;
        }

        //마지막 열린 UI 닫기
        UIManager.CloseLastOpenedUI();

        //메뉴 닫기
        if (UIManager.IsOpened<UI_Menu>())
        {
            UIManager.Hide<UI_Menu>();
            return;
        }

        //이동 취소
        if (_mousePathCoroutine != null)
        {
            StopMousePathMovement();
            return;
        }

        // 반복 이동 취소
        StopHoldMove();
    }
    public void CancelItemUse()
    {
        _isItemUsing = false;
    }
    public void DuplicationlItemUse()
    {
        _isItemUsing = true;
    }

    // ──────────── 메뉴 키(Tab) ────────────
    private void HandleMenu()
    {
        if (UIManager.IsOpened<UI_Menu>())
        {
            UIManager.Get<UI_Menu>().HideDirect();
        }
        else
        {
            UIManager.Show<UI_Menu>();
        }
    }
    //──────────── 적 검사 메서드 ────────────
    private List<CharacterStats> GetEnemiesInSight()
    {

        var enemies = new List<CharacterStats>();
        foreach (var etile in _stats.tilesOnVision)
        {
            if (etile == null) continue;
            foreach (var tile in _stats.tilesOnVision)
            {
                if (tile == null) continue;
                var stats = tile.CharacterStatsOnTile;
                if (stats != null && stats != _stats)
                    enemies.Add(stats);
            }
        }
        return enemies;
    }
    private bool HasNewEnemy(HashSet<CharacterStats> initial, List<CharacterStats> current)
    {
        if (initial == null || current == null) return false;
        foreach (var e in current)
            if (!initial.Contains(e))
                return true;
        return false;
    }

    private void StopMousePathMovement()
    {
        // 코루틴부터 멈추고
        if (_mousePathCoroutine != null)
        {
            StopCoroutine(_mousePathCoroutine);
            _mousePathCoroutine = null;
        }
        _isMouseMove = false;
        _mouseMovePath = null;
        transform.DOKill();

        //TurnManager.Instance.turnSpeed = _savedMouseTurnSpeed;
        //Time.timeScale = _savedMouseTimeScale;
        // 이동 플래그 초기화
        _isMoving = false;
    }

    private void HandleRest()
    {
        if (_isMouseMove)
        {
            StopMousePathMovement();
            if (Controller.isPlayerTurn)
            {
                UIManager.ShowOnce<UI_Text>("1턴 휴식");
                Controller.onActionConfirmed?.Invoke();
            }
            return;
        }

        // 플레이어 턴이 아니거나 이동/아이템 사용 중이면 무시
        if (!Controller.isPlayerTurn || _isMoving || _mousePathCoroutine != null || _currentItem != null)
            return;

        UIManager.ShowOnce<UI_Text>("1턴 휴식");
        Controller.onActionConfirmed?.Invoke();
    }

    private void HandleHoldMove(Vector2 raw)
    {
        if (IsUIOpen())
        {
            StopHoldMove();
            return;
        }

        if (raw == Vector2.zero)
        {
            StopHoldMove();
            return;
        }

        if (_holdMoveCoroutine != null)
            return;

        _holdMoveCoroutine = StartCoroutine(HoldMoveWithDelay(raw));
    }

    private IEnumerator HoldMoveWithDelay(Vector2 initialRaw)
    {
        float holdTimeRequired = 0.2f;
        float elapsedTime = 0f;

        // 적 검사 초기화
        _startHp = _stats.CurrentHealth;
        _initialEnemiesInSight = new HashSet<CharacterStats>(GetEnemiesInSight());

        while (elapsedTime < holdTimeRequired)
        {
            Vector2 currentInput = InputManager.Instance.MoveInput;
            if (currentInput == Vector2.zero)
            {
                _holdMoveCoroutine = null;
                yield break;
            }

            bool inputChanged = false;

            // x축 방향이 완전히 반대로 바뀌면 취소
            if ((initialRaw.x > 0 && currentInput.x < 0) ||
                (initialRaw.x < 0 && currentInput.x > 0))
            {
                inputChanged = true;
            }

            // y축 방향이 완전히 반대로 바뀌면 취소
            if ((initialRaw.y > 0 && currentInput.y < 0) ||
                (initialRaw.y < 0 && currentInput.y > 0))
            {
                inputChanged = true;
            }

            if (inputChanged)
            {
                _holdMoveCoroutine = null;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector2 finalInput = InputManager.Instance.MoveInput;
        if (finalInput != Vector2.zero)
        {
            var dir = new Vector2Int(
                finalInput.x > 0 ? 1 : finalInput.x < 0 ? -1 : 0,
                finalInput.y > 0 ? 1 : finalInput.y < 0 ? -1 : 0
            );

            if (dir != Vector2Int.zero)
            {
                StartCoroutine(HoldMove(dir));
            }
            else
            {
                _holdMoveCoroutine = null;
            }
        }
        else
        {
            _holdMoveCoroutine = null;
        }
    }

    private IEnumerator HoldMove(Vector2Int dir)
    {
        var activeHoldMove = _holdMoveCoroutine;

        while (InputManager.Instance.MoveInput != Vector2.zero)
        {
            if (_stats.CurrentHealth < _startHp ||
                HasNewEnemy(_initialEnemiesInSight, GetEnemiesInSight()))
            {
                break;
            }

            if (!_isMoving && _moveBuffer == null)
            {
                var cur = _stats.CurTile.gridPosition;
                var nxt = cur + dir;

                if (!_stats.curLevel.tiles.TryGetValue(nxt, out var tile) ||
                    !tile.IsWalkable ||
                    tile.CharacterStatsOnTile != null)
                {
                    Shake();
                    break;
                }

                ExecuteMove(dir);
                yield return new WaitUntil(() => !_isMoving);
            }
            yield return null;
        }

        if (_holdMoveCoroutine == activeHoldMove)
            _holdMoveCoroutine = null;
    }

    private void StopHoldMove()
    {
        if (_holdMoveCoroutine != null)
        {
            StopCoroutine(_holdMoveCoroutine);
            _holdMoveCoroutine = null;
        }
    }
}
