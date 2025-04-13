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

    public float MoveSpeed = 5f;         // 기본 이동 속도 (카드널 이동 기준)
    public float DashSpeed = 10f;        // 대시 속도 (참고용)
    private bool _isMoving = false;
    public float MoveActionCost = 1.0f;    // 이동 시 소모하는 행동력
    private Vector3 _targetPosition;

    public LayerMask ObstacleLayer;      // 대시 중 장애물 체크용
    public LayerMask UnitLayer;          // 플레이어 및 적들이 속한 레이어 (타일 점유 확인에 사용)

    // 마지막 이동 방향 (대시 시 사용)
    private Vector2 _lastMoveDirection = Vector2.down;

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
        // 같은 오브젝트의 PlayerStats 컴포넌트를 캐싱합니다.
        _playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();

        // 입력이 있고, 아직 이동 중이 아닐 때
        if (input != Vector2.zero && !_isMoving)
        {
            // 행동력 확인: 충분한 행동력이 있을 때만 이동 실행
            if (_playerStats != null && _playerStats.ActionPoints >= MoveActionCost)
            {
                // 마지막 이동 방향 갱신
                _lastMoveDirection = input;

                // 현재 위치의 그리드 셀 좌측 하단 좌표
                Vector3 currentCell = new Vector3(Mathf.Floor(transform.position.x),
                                                  Mathf.Floor(transform.position.y),
                                                  transform.position.z);
                Vector3 offset;
                bool isDiagonal = false;

                // 대각 이동 여부 판정
                if (Mathf.Abs(input.x) > 0.01f && Mathf.Abs(input.y) > 0.01f)
                {
                    isDiagonal = true;
                    offset = new Vector3(Mathf.Sign(input.x), Mathf.Sign(input.y), 0);
                }
                else
                {
                    offset = new Vector3(input.x, input.y, 0);
                }
                // 목표 셀 계산: 현재 셀 + offset 후 셀 중심 좌표 (0.5, 0.5)
                Vector3 targetCell = currentCell + offset;
                _targetPosition = targetCell + new Vector3(0.5f, 0.5f, 0);

                // 이동 시작 전에 목표칸에 이미 다른 유닛이 있는지 검사
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

        // 나머지 입력 처리 (확인, 취소, 메뉴, 대시)
        if (_confirmAction.triggered)
            Debug.Log("Confirm pressed");
        if (_cancelAction.triggered)
            Debug.Log("Cancel pressed");
        if (_menuAction.triggered)
            Debug.Log("Menu pressed");
        if (_dashAction.triggered)
        {
            Debug.Log("Dash activated");
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

        // 이동 후 행동력 차감 및 클램프 처리 (행동력 범위를 0~1로 유지)
        if (_playerStats != null)
        {
            _playerStats.ActionPoints -= MoveActionCost;
            _playerStats.ActionPoints = Mathf.Clamp(_playerStats.ActionPoints, 0f, 1f);
            Debug.Log("ActionPoints 소모됨: " + MoveActionCost + " 남은 행동력: " + _playerStats.ActionPoints);
        }
        yield break;
    }

    // 대시 코루틴 (타일 점유 확인 로직 추가 가능)
    IEnumerator Dash(Vector2 direction)
    {
        int maxCells = 3;  // 대시 최대 칸 수
        int cellsMoved = 0;
        bool isDiagonal = Mathf.Abs(direction.x) > 0.01f && Mathf.Abs(direction.y) > 0.01f;

        while (cellsMoved < maxCells)
        {
            if (_playerStats != null && _playerStats.ActionPoints < MoveActionCost)
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
}
