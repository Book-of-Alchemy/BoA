using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveActionCost = 1f;
    public LayerMask unitLayer;
    public LayerMask obstacleLayer;
    public float inputBufferDuration = 0.1f;
    public float dashSpeed = 10f;      // 대시 속도
    public int dashDistance = 3;     // 최대 대시 거리
    private Coroutine _dashBufferCoroutine;

    private PlayerStats _playerStats;
    private PlayerInputActions _inputActions;
    private CharacterAnimator _animator;
    private bool _isMoving;
    private bool _isCtrlHold;
    //start코루틴이 반환하는 참조들(중복실행방지, 코루틴 관리(버퍼확인)
    private Coroutine _moveBufferCoroutine;
    private Coroutine _attackBufferCoroutine;
    
    private Vector2 _lastMoveDirection = Vector2.right;// 캐릭터 방향에 따라 미리 초기화
    private SpriteRenderer _spriteRenderer;
    public event Action onActionConfirmed;//액션 선택시 실행 즉 현재 perform move perform attack 시작시 실행하면 됨
    public bool isPlayerTurn;//false일시 입력 차단
    private float _savedTurnSpeed;  // 원래 턴 속도 저장용
    private float _dashStartHealth; // 대쉬 시작전 체력 저장 필드
    private HashSet<EnemyStats> _initialVisibleEnemies;
    //>>
    //public float MoveSpeed => TurnManager.Instance.turnSpeed;
    //>>
    // BufferAndDash 후 실제 대시 스텝을 처리할 큐
    private Queue<Vector2Int> _dashQueue;
    // 대시 코루틴
    private Coroutine _dashCoroutine;

    /// <summary>
    /// input manager별도로 생성 input action은 inputmanager를 참조하도록 변경함
    /// 현재 플레이어 턴시작시 isplayerturn만 체크해주는 방식
    /// </summary>
    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _inputActions = InputManager.Instance.GetInputSafe();//lazy init을 통해 inputmanager awake 실행 전이라도 안전하게 가져올수있음
    }

    private void OnEnable()
    {
        if (_inputActions == null)
            _inputActions = InputManager.Instance.GetInputSafe();

        _inputActions.PC.Move.started += OnMoveStarted;
        _inputActions.PC.Attack.started += OnAttackStarted;
        _inputActions.PC.Dash.started += OnDashStarted;
        _inputActions.PC.Ctrl.performed += OnCtrlPressed;
        _inputActions.PC.Ctrl.canceled += OnCtrlReleased;
    }

    public void OnDisable()
    {
        _inputActions.PC.Move.started -= OnMoveStarted;
        _inputActions.PC.Attack.started -= OnAttackStarted;
        _inputActions.PC.Dash.started -= OnDashStarted;
        _inputActions.PC.Ctrl.performed -= OnCtrlPressed;
        _inputActions.PC.Ctrl.canceled -= OnCtrlReleased;
    }
    private void OnCtrlPressed(InputAction.CallbackContext ctx) => _isCtrlHold = true;
    private void OnCtrlReleased(InputAction.CallbackContext ctx) => _isCtrlHold = false;

    /// <summary>
    /// 구독 및 해제를 해줘야하므로 람다식으로 처리시 구독해제가 불가함
    /// 따라서 람다식 부분 메서드로 옮김 
    /// buffmanager 사라진 관계로 주석처리
    /// </summary>
    /// <param name="ctx"></param>
    private void OnMoveStarted(InputAction.CallbackContext ctx)
    {
        if (!isPlayerTurn) 
            return;
        if (!_isMoving && _moveBufferCoroutine == null)
        {
            _moveBufferCoroutine = StartCoroutine(BufferAndMove());
        }
    }

    private void OnAttackStarted(InputAction.CallbackContext ctx)
    {
        if (!isPlayerTurn)
            return;
        if (_attackBufferCoroutine == null)
        {
            _attackBufferCoroutine = StartCoroutine(BufferAndAttack());
        }
    }

    private void OnDashStarted(InputAction.CallbackContext ctx)
    {
        if (!isPlayerTurn || _dashBufferCoroutine != null) return;
        _dashBufferCoroutine = StartCoroutine(BufferAndDash());
    }

    //이동 버퍼 코루틴
    private IEnumerator BufferAndMove()
    {
        float elapsed = 0f;//경과시간
        Vector2 bufferedInput = Vector2.zero;// 0으로 초기화

        //Move 액션의 값을 읽고 (0,0)이 아닌 입력이 들어올때마다 버퍼 인풋 갱신
        while (elapsed < inputBufferDuration)//elapsed가 0.1초가 될때까지 반복
        {
            Vector2 current = _inputActions.PC.Move.ReadValue<Vector2>();
            if (current != Vector2.zero) bufferedInput = current;
            elapsed += Time.deltaTime;
            yield return null;
        }

        //최종 입력처리
        if (bufferedInput != Vector2.zero)
        {
            ExecuteMove(bufferedInput);
        }
        
        //코루틴을 마치고 참조를 null로 되돌림
        _moveBufferCoroutine = null;
    }

    private IEnumerator BufferAndAttack()
    {
        float elapsed = 0f;
        Vector2 bufferedInput = Vector2.zero;

        while (elapsed < inputBufferDuration)
        {
            Vector2 current = _inputActions.PC.Move.ReadValue<Vector2>();
            if (current != Vector2.zero) bufferedInput = current;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 애니메이션만 재생 
        _animator.PlayAttack();
        onActionConfirmed?.Invoke();

        _attackBufferCoroutine = null;
    }



    private IEnumerator BufferAndDash()
    {
        float elapsed = 0f;
        Vector2 buf = Vector2.zero;
        while (elapsed < 0.1f)
        {
            Vector2 cur = InputManager.Instance.GetInputSafe().PC.Move.ReadValue<Vector2>();
            if (cur != Vector2.zero) buf = cur;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2) 방향 & 최대 칸 수 계산
        Vector2 raw = buf != Vector2.zero ? buf : _lastMoveDirection;
        Vector2Int offset = new Vector2Int(
            raw.x > 0 ? 1 : raw.x < 0 ? -1 : 0,
            raw.y > 0 ? 1 : raw.y < 0 ? -1 : 0
        );
        _lastMoveDirection = offset;

        var startPos = _playerStats.CurTile.gridPosition;
        int maxSteps = 0;
        for (int i = 1; i <= dashDistance; i++)
        {
            var next = startPos + offset * i;
            if (!_playerStats.curLevel.tiles.TryGetValue(next, out var t)) break;
            if (!t.IsWalkable || t.CharacterStatsOnTile != null) break;
            maxSteps = i;
        }

        if (maxSteps > 0)
        {
            //대시 시작 전 상태 저장
            _dashStartHealth = _playerStats.CurrentHealth;
            _initialVisibleEnemies = new HashSet<EnemyStats>();
            foreach (var tile in _playerStats.tilesOnVision)
                if (tile.CharacterStatsOnTile is EnemyStats e)
                    _initialVisibleEnemies.Add(e);

            var tm = TurnManager.Instance;
            _savedTurnSpeed = tm.turnSpeed;
            tm.turnSpeed *= 10f;
            moveSpeed *= 10f;
            Time.timeScale *= 10f;
            // 큐에 스텝 담기
            _dashQueue = new Queue<Vector2Int>();
            for (int i = 0; i < maxSteps; i++)
                _dashQueue.Enqueue(offset);

            // 대시 코루틴 시작
            _dashCoroutine = StartCoroutine(DashCoroutine());
        }

        _dashBufferCoroutine = null;
    }


    private void ExecuteMove(Vector2 rawInput)
    {
        //현재 플레이어가 서있는 타일의 격자 좌표 가져오기
        Vector2Int curCell = _playerStats.CurTile.gridPosition;

        //rawInput(–1~1 사이 실수 값)을 –1, 0, 1 중 하나로 변환해 격자 한칸 단위 이동 방향(offset)을 결정
        Vector2Int offset = new Vector2Int(
            rawInput.x > 0 ? 1 : rawInput.x < 0 ? -1 : 0,
            rawInput.y > 0 ? 1 : rawInput.y < 0 ? -1 : 0
        );

        if (_isCtrlHold)
        {
            if (offset.x != 0)
                _spriteRenderer.flipX = offset.x < 0;
            _lastMoveDirection = offset;
            return;
        }
        //스프라이트 flipx
        if (offset.x != 0)
            _spriteRenderer.flipX = offset.x < 0;

        //마지막 이동방향 저장
        _lastMoveDirection = offset;
        //목표 좌표 계산
        Vector2Int tgtCell = curCell + offset;
        _spriteRenderer.sortingOrder = -tgtCell.y * 10 + 1;
        //이동 가능한지 판별
        if (!_playerStats.curLevel.tiles.TryGetValue(tgtCell, out var tile)
            || tile.CharacterStatsOnTile != null
            || !tile.IsWalkable)
            return;

        _isMoving = true;
        _playerStats.CurTile.CharacterStatsOnTile = null;// 전 타일 null로 비우기
        
        // 새 타일에 등록
        _playerStats.CurTile = tile;
        tile.CharacterStatsOnTile = _playerStats;

        //월드 좌표로 목적지(dest) 계산후 지정된 이속으로 걸리는 시간 구하기
        Vector3 dest = new Vector3(tgtCell.x, tgtCell.y, 0f);
        float duration = 0.1f;
        onActionConfirmed?.Invoke();
        //이동 애니메이션 재생및 움직임(행동력 소모)
        _animator.PlayMove();
        transform
            .DOMove(dest, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isMoving = false;
                //_playerStats.BuffManager.ApplyBuff(-MoveActionCost, 0);
            });
    }
    public void OnAttackHit()
    {
        DoAttack(_lastMoveDirection);
    }
    private void DoAttack(Vector2 dir)
    {
        if (dir.x != 0)
            _spriteRenderer.flipX = dir.x < 0;
        PerformAttackRaycast(dir);//지정된 방향으로 레이발사
        //_playerStats.BuffManager.ApplyBuff(-MoveActionCost, 0);
    }

    private void PerformAttackRaycast(Vector2 dir)
    {
        onActionConfirmed?.Invoke();
        //시작점을 콜라이더 바깥으로 살싹 떨어뜨리기 extents는 반지름
        var col = GetComponent<Collider2D>();
        float originOffset = col.bounds.extents.magnitude + 0.1f;

        // nomalized를 통한 정규화된 공격방향을 캐릭터 중심 위치와 곱하여 레이 출발지점 계산
        Vector2 origin = (Vector2)transform.position + dir.normalized * originOffset;
        Vector2 direction = dir.normalized;// 중복방지를 위해 방향을 한번더 저장

        Debug.DrawRay(origin, direction * 0.5f, Color.red, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 0.5f, unitLayer);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                _playerStats.Attack(enemyStats);
            }
        }
    }
    //대시 실행 메서드
    private IEnumerator DashCoroutine()
    {
        bool cancelled = false;
        bool firstStep = true;
        var tm = TurnManager.Instance;

        while (_dashQueue.Count > 0 && !cancelled)
        {
            // 첫 스텝 전에는 대기 없이 바로 이동
            if (!firstStep)
                yield return new WaitUntil(() => isPlayerTurn);
            firstStep = false;

            //  한칸씩 이동 처리
            Vector2Int step = _dashQueue.Dequeue();
            Vector2Int cur = _playerStats.CurTile.gridPosition;
            Vector2Int next = cur + step;

            if (!_playerStats.curLevel.tiles.TryGetValue(next, out var tile)
                || !tile.IsWalkable
                || tile.CharacterStatsOnTile != null|| _playerStats.CurrentHealth < _dashStartHealth)
                break;

            // 방향 및 애니메이션
            if (step.x != 0) _spriteRenderer.flipX = step.x < 0;
            _animator.PlayMove();

            // 타일 점유 갱신
            _playerStats.CurTile.CharacterStatsOnTile = null;
            _playerStats.CurTile = tile;
            tile.CharacterStatsOnTile = _playerStats;

            // Tween 이동
            Vector3 dest = new Vector3(next.x, next.y, 0f);
            float duration = 0.01f;
            yield return transform
                .DOMove(dest, duration)
                .SetEase(Ease.Linear)
                .WaitForCompletion();
            //

            //// 행동력 차감 + 턴 종료 알림
            //int stepCost = /* 스텝당 코스트 계산 */;
            //GetComponent<PlayerUnit>().SetNextActionCost(stepCost);
            onActionConfirmed?.Invoke();
            // 새로 보이는 적
            foreach (var vis in _playerStats.tilesOnVision)
            {
                if (vis.CharacterStatsOnTile is EnemyStats e
                    && !_initialVisibleEnemies.Contains(e))
                {
                    cancelled = true;
                    break;
                }
            }
        }

        // 마지막 스텝 후에도 적에게 한 번 기회를 주기 위해 대기
        yield return new WaitUntil(() => isPlayerTurn);

        // 대시 종료 후 턴 속도 복구
        tm.turnSpeed = _savedTurnSpeed;
        _dashCoroutine = null;
        moveSpeed = moveSpeed / 10;
        Time.timeScale /= 10f;
    }
}
