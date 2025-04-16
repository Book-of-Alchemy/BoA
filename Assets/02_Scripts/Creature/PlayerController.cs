using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    void Awake()
    {
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

        // 컨트롤 키가 눌려 있지 않은 경우에만 이동 로직 실행
        if (!Keyboard.current.leftCtrlKey.isPressed)
        {
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            if (moveInput != Vector2.zero && !_isMoving)
            {
                // 행동력 확인: 충분한 행동력이 있을 때만 이동 실행
                if (_playerStats != null && _playerStats.ActionPoints >= MoveActionCost)
                {
                    // 마지막 이동 방향 갱신
                    _lastMoveDirection = moveInput;

                    // 현재 그리드 셀 계산 (바닥 좌표)
                    Vector3 currentCell = new Vector3(Mathf.Floor(transform.position.x),
                                                      Mathf.Floor(transform.position.y),
                                                      transform.position.z);
                    Vector3 offset;
                    bool isDiagonal = false;

                    // 대각 이동 여부 판정
                    if (Mathf.Abs(moveInput.x) > 0.01f && Mathf.Abs(moveInput.y) > 0.01f)
                    {
                        isDiagonal = true;
                        offset = new Vector3(Mathf.Sign(moveInput.x), Mathf.Sign(moveInput.y), 0);
                    }
                    else
                    {
                        offset = new Vector3(moveInput.x, moveInput.y, 0);
                    }
                    // 목표 셀 계산: 현재 셀 + offset 후 셀 중심 좌표 (0.5, 0.5)
                    Vector3 targetCell = currentCell + offset;
                    _targetPosition = targetCell + new Vector3(0.5f, 0.5f, 0);

                    // 이동 시작 전에 목표 칸에 이미 다른 유닛이 있는지 검사
                    Collider2D hit = Physics2D.OverlapPoint(_targetPosition, UnitLayer);
                    if (hit != null && hit.gameObject != gameObject)
                    {
                        Debug.Log("타일이 다른 유닛에 의해 점유되어 이동할 수 없습니다.");
                        return;
                    }

                    // 이동 코루틴 실행
                    StartCoroutine(MoveToTarget(_targetPosition, isDiagonal));
                }
                else
                {
                    Debug.Log("행동력이 부족하여 이동할 수 없습니다.");
                }
            }
        }

        // 취소, 메뉴, 대시 입력 처리
        if (_cancelAction.triggered)
            Debug.Log("취소 눌림");
        if (_menuAction.triggered)
            Debug.Log("메뉴 눌림");
        if (_dashAction.triggered)
        {
            Debug.Log("대쉬 활성화");
            StartCoroutine(Dash(_lastMoveDirection));
        }
    }

    // 목표 위치까지 부드럽게 이동하는 코루틴
    IEnumerator MoveToTarget(Vector3 destination, bool isDiagonal = false)
    {
        _isMoving = true;
        float effectiveSpeed = MoveSpeed;

        while ((destination - transform.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, effectiveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destination;
        _isMoving = false;

        // 이동 후 행동력 차감 및 계산 (버프 시스템을 이용하여 소비)
        if (_playerStats != null)
        {
            // 행동력 소비: 이동과 같은 코스트만큼 소비 후 BuffManager에서 최종 AP 재계산
            _playerStats.BuffManager.ApplyBuff(-MoveActionCost, 0); // 지속 턴 0은 즉시 소비
            Debug.Log("ActionPoints 소모됨: " + MoveActionCost + ", 남은 AP: " + _playerStats.BuffManager.GetFinalActionPoints());
        }
        yield break;
    }

    // 대시 코루틴
    IEnumerator Dash(Vector2 direction)
    {
        int maxCells = 3;  // 대시 최대 칸 수
        int cellsMoved = 0;
        bool isDiagonal = Mathf.Abs(direction.x) > 0.01f && Mathf.Abs(direction.y) > 0.01f;

        while (cellsMoved < maxCells)
        {
            if (_playerStats != null && _playerStats.BuffManager.GetFinalActionPoints() < MoveActionCost)
            {
                Debug.Log("행동력이 부족하여 대시 중단");
                break;
            }

            Vector3 currentCell = new Vector3(Mathf.Floor(transform.position.x),
                                              Mathf.Floor(transform.position.y),
                                              transform.position.z);
            Vector3 nextCell = currentCell + new Vector3(Mathf.Sign(direction.x), Mathf.Sign(direction.y), 0);
            Vector3 destination = nextCell + new Vector3(0.5f, 0.5f, 0);

            // 대시 시에도 목적지 칸 점유 확인
            Collider2D hit = Physics2D.OverlapPoint(destination, UnitLayer);
            if (hit != null && hit.gameObject != gameObject)
            {
                Debug.Log("대시 중인 칸이 다른 유닛에 의해 점유되어 대시 중단");
                break;
            }

            if (Physics2D.OverlapPoint(destination, ObstacleLayer))
                break;

            yield return StartCoroutine(MoveToTarget(destination, isDiagonal));
            cellsMoved++;
        }
        yield break;
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
}
