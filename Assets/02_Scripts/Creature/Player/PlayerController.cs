using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float MoveActionCost = 1f;
    public LayerMask UnitLayer;
    public LayerMask ObstacleLayer;
    public float InputBufferDuration = 0.1f;

    private PlayerStats _playerStats;
    private PlayerInputActions _inputActions;
    private CharacterAnimator _animator;
    private bool _isMoving;
    //start코루틴이 반환하는 참조들(중복실행방지, 코루틴 관리(버퍼확인)
    private Coroutine _moveBufferCoroutine;
    private Coroutine _attackBufferCoroutine;

    private Vector2 _lastMoveDirection = Vector2.right;// 캐릭터 방향에 따라 미리 초기화
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _inputActions = new PlayerInputActions();

        // 콜백 등록
        _inputActions.PC.Move.started += ctx =>
        {
            if (!_isMoving
                && _playerStats.BuffManager.GetFinalActionPoints() >= MoveActionCost
                && _moveBufferCoroutine == null)
            {
                _moveBufferCoroutine = StartCoroutine(BufferAndMove());
            }
        };

        _inputActions.PC.Attack.started += ctx =>
        {
            if (_attackBufferCoroutine == null)
            {
                _attackBufferCoroutine = StartCoroutine(BufferAndAttack());
            }
        };
    }
    // 중복 이벤트 방지를 위해 PC맵을 껐다 키는 메서드
    private void OnEnable() => _inputActions.PC.Enable();
    public void OnDisable() => _inputActions.PC.Disable();

    //이동 버퍼 코루틴
    private IEnumerator BufferAndMove()
    {
        float elapsed = 0f;//경과시간
        Vector2 bufferedInput = Vector2.zero;// 0으로 초기화

        //Move 액션의 값을 읽고 (0,0)이 아닌 입력이 들어올때마다 버퍼 인풋 갱신
        while (elapsed < InputBufferDuration)//elapsed가 0.1초가 될때까지 반복
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

        while (elapsed < InputBufferDuration)
        {
            Vector2 current = _inputActions.PC.Move.ReadValue<Vector2>();
            if (current != Vector2.zero) bufferedInput = current;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 버퍼 시간에 방향키를 눌렀다면 그 방향으로 버퍼인풋, 안눌렀다면 마지막 이동방향으로
        Vector2 rawInput = bufferedInput != Vector2.zero ? bufferedInput : _lastMoveDirection;
        Vector2Int offset = new Vector2Int(
            rawInput.x > 0 ? 1 : rawInput.x < 0 ? -1 : 0,
            rawInput.y > 0 ? 1 : rawInput.y < 0 ? -1 : 0
        );
        //공격방향 업데이트 및 실제 공격 호출
        _lastMoveDirection = offset;
        _animator.PlayAttack();

        _attackBufferCoroutine = null;
    }

    private void ExecuteMove(Vector2 rawInput)
    {
        //현재 플레이어가 서있는 타일의 격자 좌표 가져오기
        Vector2Int curCell = _playerStats.curTile.gridPosition;

        //rawInput(–1~1 사이 실수 값)을 –1, 0, 1 중 하나로 변환해 격자 한칸 단위 이동 방향(offset)을 결정
        Vector2Int offset = new Vector2Int(
            rawInput.x > 0 ? 1 : rawInput.x < 0 ? -1 : 0,
            rawInput.y > 0 ? 1 : rawInput.y < 0 ? -1 : 0
        );
        
        //스프라이트 flipx
        if (offset.x != 0)
            _spriteRenderer.flipX = offset.x < 0;

        //마지막 이동방향 저장
        _lastMoveDirection = offset;
        //목표 좌표 계산
        Vector2Int tgtCell = curCell + offset;

        //이동 가능한지 판별
        if (!_playerStats.curLevel.tiles.TryGetValue(tgtCell, out var tile)
            || tile.CharacterStatsOnTile != null
            || !tile.IsWalkable)
            return;

        _isMoving = true;
        _playerStats.curTile.CharacterStatsOnTile = null;// 전 타일 null로 비우기
        
        // 새 타일에 등록
        _playerStats.curTile = tile;
        tile.CharacterStatsOnTile = _playerStats;

        //월드 좌표로 목적지(dest) 계산후 지정된 이속으로 걸리는 시간 구하기
        Vector3 dest = new Vector3(tgtCell.x, tgtCell.y, 0f);
        float duration = Vector3.Distance(transform.position, dest) / MoveSpeed;

        //이동 애니메이션 재생및 움직임(행동력 소모)
        _animator.PlayMove();
        transform
            .DOMove(dest, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isMoving = false;
                _playerStats.BuffManager.ApplyBuff(-MoveActionCost, 0);
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
        _playerStats.BuffManager.ApplyBuff(-MoveActionCost, 0);
    }

    private void PerformAttackRaycast(Vector2 dir)
    {
        //시작점을 콜라이더 바깥으로 살싹 떨어뜨리기 extents는 반지름
        var col = GetComponent<Collider2D>();
        float originOffset = col.bounds.extents.magnitude + 0.1f;

        // nomalized를 통한 정규화된 공격방향을 캐릭터 중심 위치와 곱하여 레이 출발지점 계산
        Vector2 origin = (Vector2)transform.position + dir.normalized * originOffset;
        Vector2 direction = dir.normalized;// 중복방지를 위해 방향을 한번더 저장

        Debug.DrawRay(origin, direction * 0.5f, Color.red, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 0.5f, UnitLayer);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                _playerStats.Attack(enemyStats);
            }
        }
    }
}
