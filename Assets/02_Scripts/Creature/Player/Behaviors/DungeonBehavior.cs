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

    // 마지막 이동 방향 (하이라이트용)
    private Vector2Int _lastMoveDir = Vector2Int.right;

    // 달리기(Shift) 상태 저장
    private float _savedTurnSpeed;

    // 하이라이트 인스턴스
    private GameObject _highlightInstance;

    // 아이템 사용
    private BaseItem _currentItem;

    // 마우스 경로 이동 시 시간 조절용 필드
    private float _savedMouseTimeScale;
    private float _savedMouseTurnSpeed;

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

        if (!Controller.isPlayerTurn || _isMoving)
            return;

        if (_mousePathCoroutine != null)
        {
            StopMousePathMovement();
        }

        Vector2Int goalPos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.y)
        );

        if (!_stats.curLevel.tiles.TryGetValue(goalPos, out Tile goalTile))
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
        List<Tile> path = AstarPlayerPathFinder.FindPath(_stats.CurTile, goalTile, _stats.curLevel);
        if (path == null || path.Count <= 1)
            return;

        // 시간 스케일 5배 적용
        //_savedMouseTimeScale = Time.timeScale;
        //_savedMouseTurnSpeed = TurnManager.Instance.turnSpeed;
        //Time.timeScale = _savedMouseTimeScale * 5f;
        //TurnManager.Instance.turnSpeed = _savedMouseTurnSpeed * 5f;

        // 이동 중단 검사 초기화
        _startHp = _stats.CurrentHealth;
        _initialEnemiesInSight = new HashSet<CharacterStats>(GetEnemiesInSight());

        _mousePathCoroutine = StartCoroutine(MoveAlongPath(path));
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
    private IEnumerator MoveAlongPath(List<Tile> path)
    {
        // 현재 위치 제외
        path.RemoveAt(0);

        foreach (var tile in path)
        {
            // 이전 이동이 끝날 때까지 대기
            yield return new WaitUntil(() => !_isMoving);

            // 한 칸 이동 실행
            Vector2Int dir = tile.gridPosition - _stats.CurTile.gridPosition;
            ExecuteMove(dir);

            // 이동 애니메이션이 완전히 끝날 때까지 대기
            yield return new WaitUntil(() => !_isMoving);

            // 턴 매니저가 플레이어 턴을 다시 열어줄 때까지 대기
            yield return new WaitUntil(() => Controller.isPlayerTurn);

            // 중단 조건 검사
            if (_stats.CurrentHealth < _startHp ||
                HasNewEnemy(_initialEnemiesInSight, GetEnemiesInSight()))
            {
                break;
            }
        }
        //TurnManager.Instance.turnSpeed = _savedMouseTurnSpeed;
        //Time.timeScale = _savedMouseTimeScale;

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
            Vector2 raw = InputManager.MoveInput;
            if (raw.x != 0) rawBuf.x = raw.x;
            if (raw.y != 0) rawBuf.y = raw.y;

            elapsed += Time.deltaTime;
            yield return null;
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
        if (!Controller.isPlayerTurn||
            _isMoving ||
            _moveBuffer != null ||
            raw == Vector2.zero)
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

        transform
            .DOMove(new Vector3(nxt.x, nxt.y, 0),1/(Controller.moveSpeed*3))
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                // 피격 또는 신규 적 발견 시 즉시 중단
                if (_stats.CurrentHealth < _startHp ||
                    HasNewEnemy(_initialEnemiesInSight, GetEnemiesInSight()))
                {
                    StopMousePathMovement();
                }
            })
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
        _animator.PlayAttack();
        
        _attackBuffer = null;
    }
    public void OnAttackHit()
    {
        Vector2Int targetPos = _stats.CurTile.gridPosition + _lastMoveDir;
        if (_stats.curLevel.tiles.TryGetValue(targetPos, out Tile tile)
            && tile.CharacterStatsOnTile != null)
        {
            _stats.Attack(tile.CharacterStatsOnTile);
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
        HideHighlight();
    }

    private IEnumerator HighlightWithBuffer()
    {
        while (_isCtrlHeld)
        {
            yield return BufferInput(dir =>
            {
                _lastMoveDir = dir;
                UpdateHighlightPosition();
            });
        }
        HideHighlight();
    }

    private void UpdateHighlightPosition()
    {
        var cur = _stats.CurTile.gridPosition;
        var tgt = cur + _lastMoveDir;

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

        _currentItem.ItemUseDone -= HandleItemUseDone;
        Controller.onActionConfirmed?.Invoke();
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
        transform.DOKill();

        //TurnManager.Instance.turnSpeed = _savedMouseTurnSpeed;
        //Time.timeScale = _savedMouseTimeScale;
        // 이동 플래그 초기화
        _isMoving = false;
    }

    private void HandleRest()
    {
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
        float holdTimeRequired = 0.5f;
        float elapsedTime = 0f;
        
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
        
        bool isHoldMoveStarted = false;
        
        yield return BufferInput(dir => {
            if (dir != Vector2Int.zero)
            {
                isHoldMoveStarted = true;
                StartCoroutine(HoldMove(dir));
            }
        });
        
        if (!isHoldMoveStarted)
        {
            _holdMoveCoroutine = null;
        }
    }

    private IEnumerator HoldMove(Vector2Int dir)
    {
        var activeHoldMove = _holdMoveCoroutine;

        while (InputManager.Instance.MoveInput != Vector2.zero)
        {
            if (Controller.isPlayerTurn && !_isMoving)
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
