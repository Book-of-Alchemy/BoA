using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private InputAction _moveAction;
    private InputAction _confirmAction;
    private InputAction _cancelAction;
    private InputAction _menuAction;
    private InputAction _dashAction;

    public float MoveSpeed = 5f;         // 기본 이동 속도
    public float DashSpeed = 10f;        // 대시 속도
    private bool _isMoving = false;
    public float MoveActionCost = 1.0f;    // 이동 시 소모하는 행동력
    private Vector3 _targetPosition;

    public LayerMask ObstacleLayer;      // 대시 중 장애물 체크용
    public LayerMask UnitLayer;          // 플레이어 및 적들이 속한 레이어

    // 마지막 이동 방향 (대시 시 사용)
    private Vector2 _lastMoveDirection = Vector2.down;
    // 공격 모드에서 공격 방향 결정용 변수
    private Vector2 _attackDirection = Vector2.zero;

    private PlayerStats _playerStats;
    // 입력 버퍼링 중인지 표시
    private bool _isBuffering = false;
    // 버퍼링 지속 시간 (초)
    public float InputBufferDuration = 0.1f;
    //애니메이션 관련 필드
    private CharacterAnimator _anim;
    private Vector2 _moveInput;
    private SpriteRenderer _spriteRenderer;
    /*죽음과 넉백 애니메이션 호출은 캐릭터 스탯에 있음*/
    void Awake()
    {
        //애니메이션 관련 캐싱
        _anim = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // 이동 액션 (WASD, 2D 벡터 조합 - 대각 입력도 가능)
        _moveAction = new InputAction("Move", InputActionType.Value, binding: "2DVector");
        _moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // 확인, 취소, 메뉴, 대시 액션 설정
        _confirmAction = new InputAction("Confirm", binding: "<Keyboard>/z");
        _confirmAction.AddBinding("<Keyboard>/enter");
        _cancelAction = new InputAction("Cancel", binding: "<Keyboard>/x");
        _cancelAction.AddBinding("<Keyboard>/escape");
        _menuAction = new InputAction("Menu", binding: "<Keyboard>/tab");
        _dashAction = new InputAction("Dash", binding: "<Keyboard>/leftShift");
    }

    void OnEnable()
    {
        _moveAction.Enable();
        _confirmAction.Enable();
        _cancelAction.Enable();
        _menuAction.Enable();
        _dashAction.Enable();
    }

    void OnDisable()
    {
        _moveAction.Disable();
        _confirmAction.Disable();
        _cancelAction.Disable();
        _menuAction.Disable();
        _dashAction.Disable();
    }

    void Start()
    {
        // 같은 오브젝트의 PlayerStats 컴포넌트를 캐싱
        _playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        //좌우 플립
        if (Mathf.Abs(_moveInput.x) > 0.01f)
            _spriteRenderer.flipX = _moveInput.x < 0f;

        // 왼쪽 컨트롤 키가 눌렸다면 방향 입력만 받아서 공격 방향 업데이트
        if (Keyboard.current.leftCtrlKey.isPressed)
        {
            Vector2 input = _moveAction.ReadValue<Vector2>();
            if (input != Vector2.zero)
            {
                _attackDirection = input;
                _lastMoveDirection = input;  // 마지막 이동 방향 업데이트
            }
        }

        // 공격 실행은 컨트롤 키와 무관하게 확인 액션(Z 또는 Enter)로 처리
        if (_confirmAction.triggered)
        {
            // 설정된 공격 방향이 없으면 마지막 이동 방향을 사용
            Vector2 attackDir = _attackDirection != Vector2.zero ? _attackDirection : _lastMoveDirection;
            AttackInDirection(attackDir);
        }

        if (!Keyboard.current.leftCtrlKey.isPressed && !_isMoving)
        {
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            if (moveInput != Vector2.zero && !_isBuffering)
            {
                if (_playerStats != null && _playerStats.BuffManager.GetFinalActionPoints() >= MoveActionCost)
                {
                    StartCoroutine(BufferAndMove(moveInput));
                }
                else
                {
                    Debug.Log("행동력이 부족하여 이동할 수 없습니다.");
                }
            }
        }
        //이동 여부에 따라 애니메이터에 값 전달
        _isMoving = _moveInput.sqrMagnitude > 0.01f;
        _anim.SetMoving(_isMoving);
        // 취소, 메뉴, 대시 입력 처리
        if (_cancelAction.triggered)
            Debug.Log("취소 눌림");
        if (_menuAction.triggered)
            Debug.Log("메뉴 눌림");
        if (_dashAction.triggered)
        {
            Debug.Log("대쉬 활성화");
            // 대시 메서드 실행할 장소
        }
    }

    // 공격 방향에 따라 공격 실행 (Raycast를 사용)
    void AttackInDirection(Vector2 direction)
    {
        // 공격 방향을 단위 벡터로 만듦
        Vector2 attackDir = direction.normalized;

        Collider2D myCollider = GetComponent<Collider2D>();
        float offsetDistance = myCollider != null ? myCollider.bounds.extents.magnitude + 0.1f : 0.1f;

        // 오프셋을 적용하여 레이 시작점 결정
        Vector2 origin = (Vector2)transform.position + attackDir * offsetDistance;

        // 레이 길이 설정
        float rayDistance = 0.5f;

        // Raycast 실행
        RaycastHit2D hit = Physics2D.Raycast(origin, attackDir, rayDistance, UnitLayer);
        Debug.DrawRay(origin, attackDir * rayDistance, Color.red, 1f);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                _playerStats.Attack(enemyStats);
                // 공격 후 행동력 소비 (이동과 같은 코스트)
                _playerStats.BuffManager.ApplyBuff(-MoveActionCost, 0);
                Debug.Log("공격 후 행동력 소비됨: " + MoveActionCost + ", 남은 AP: " + _playerStats.BuffManager.GetFinalActionPoints());
            }
            else
            {
                Debug.Log("공격 대상이 아님");
            }
        }
        else
        {
            Debug.Log("공격할 적이 없음.");
        }
    }
    //이동 버퍼링 코루틴
    private IEnumerator BufferAndMove(Vector2Int initialInput)
    {
        _isBuffering = true;
        float elapsed = 0f;

        Vector2Int bufferedInput = initialInput;

        while (elapsed < InputBufferDuration)
        {
            Vector2Int input = _moveAction.ReadValue<Vector2Int>();
            if (input != Vector2.zero)
                bufferedInput = input; //새 입력이 있으면 그전 입력을 덮어씀

            elapsed += Time.deltaTime;
            yield return null;
        }

        _isBuffering = false;

        _lastMoveDirection = bufferedInput;
        ExecuteMove(bufferedInput);
    }

    private void ExecuteMove(Vector2 moveInput)
    {
        //캐릭터 위치 계산
        Vector2Int currentCell = _playerStats.curTile.gridPosition;

        //대각방향 구분
        Vector3 drcell;
        if (Mathf.Abs(moveInput.x) > 0.01f && Mathf.Abs(moveInput.y) > 0.01f)
            drcell = new Vector3(Mathf.Sign(moveInput.x), Mathf.Sign(moveInput.y), 0);
        else
            drcell = new Vector3(moveInput.x, moveInput.y, 0);

        Vector2Int targetCell = Vector2Int.RoundToInt(currentCell + drcell);

        Collider2D hit = Physics2D.OverlapPoint(_targetPosition, UnitLayer);
        if (!_playerStats.curLevel.tiles.TryGetValue(targetCell, out Tile targetTile))
        {
            Debug.LogError($"해당 위치에 Tile이 없습니다: {targetCell}");
            return;
        }
        //타일 점유 정보 갱신
        _playerStats.MoveToTile(targetTile);

        // 실제이동 실행
        Vector3 dest = new Vector3(targetCell.x, targetCell.y, 0f);
        float distance = Vector3.Distance(transform.position, dest);
        float duration = distance / MoveSpeed;
        _isMoving = true;

        transform
            .DOMove(dest, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isMoving = false;
                _playerStats.BuffManager.ApplyBuff(-MoveActionCost, 0);
                Debug.Log($"ActionPoints 소모됨: {MoveActionCost}, 남은 AP: {_playerStats.BuffManager.GetFinalActionPoints()}");
            });
    }
}
