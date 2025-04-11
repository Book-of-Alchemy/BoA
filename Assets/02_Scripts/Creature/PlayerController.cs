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

    public float MoveSpeed = 5f;         // 이동 속도
    public float DashSpeed = 10f;        // 대시 속도
    private bool _isMoving = false;
    public float MoveActionCost = 1.0f;    // 이동 시 소모하는 행동력
    private Vector3 _targetPosition;

    public LayerMask ObstacleLayer;      // 대시 중 장애물 체크용

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

        // 확인 액션 (Z와 엔터)
        _confirmAction = new InputAction("Confirm", binding: "<Keyboard>/z");
        _confirmAction.AddBinding("<Keyboard>/enter");

        // 취소 액션 (X와 ESC)
        _cancelAction = new InputAction("Cancel", binding: "<Keyboard>/x");
        _cancelAction.AddBinding("<Keyboard>/escape");

        // 메뉴 호출 액션 (Tab)
        _menuAction = new InputAction("Menu", binding: "<Keyboard>/tab");

        // 대시 액션 (왼쪽 Shift)
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
        _playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();

        // 행동력 확인: 이동 시작 전에 PlayerStats의 행동력 확인
        if (input != Vector2.zero && !_isMoving)
        {
            if (_playerStats != null && _playerStats.ActionPoints >= MoveActionCost)
            {
                // 마지막 이동 방향 갱신
                _lastMoveDirection = input;

                // 현재 위치의 그리드 셀 좌측 하단 좌표
                Vector3 currentCell = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), transform.position.z);
                Vector3 offset;
                bool isDiagonal = false;

                // x, y 모두 0이 아니면 대각 이동으로 판단
                if (Mathf.Abs(input.x) > 0.01f && Mathf.Abs(input.y) > 0.01f)
                {
                    isDiagonal = true;
                    offset = new Vector3(Mathf.Sign(input.x), Mathf.Sign(input.y), 0);
                }
                else
                {
                    offset = new Vector3(input.x, input.y, 0);
                }
                // 최종 목적지: 현재 셀에 offset 후, 셀 중심 오프셋 (0.5, 0.5)
                Vector3 targetCell = currentCell + offset;
                _targetPosition = targetCell + new Vector3(0.5f, 0.5f, 0);

                // 대각 이동일 경우에도 이동 속도를 그대로 사용함 (필요에 따라 조정 가능)
                StartCoroutine(MoveToTarget(_targetPosition, isDiagonal));
            }
            else
            {
                Debug.Log("행동력이 부족하여 이동할 수 없습니다.");
            }
        }

        if (_confirmAction.triggered)
            Debug.Log("Confirm pressed"); // 상호작용,아이템 사용, 대화,메뉴 진입 등

        if (_cancelAction.triggered)
            Debug.Log("Cancel pressed");  // 선택 취소, 메뉴 닫기, 대화스킵 등

        if (_menuAction.triggered)
            Debug.Log("Menu pressed");    // 인벤토리/제작메뉴 호출

        if (_dashAction.triggered)
        {
            Debug.Log("Dash activated");
            StartCoroutine(Dash(_lastMoveDirection));
        }
    }

    IEnumerator MoveToTarget(Vector3 destination, bool isDiagonal = false)
    {
        _isMoving = true;
        float effectiveSpeed = MoveSpeed; // 필요시 이동 속도를 조정

        while ((destination - transform.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, effectiveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destination;
        _isMoving = false;

        // 이동이 끝난 후 행동력 차감
        if (_playerStats != null)
        {
            _playerStats.ActionPoints -= MoveActionCost;
            Debug.Log("ActionPoints 소모됨: " + MoveActionCost + " 남은 행동력: " + _playerStats.ActionPoints);
        }
        yield break;
    }

    // 대시도 동일하게 행동력 조건을 따르고, 여러 칸 연속 이동
    IEnumerator Dash(Vector2 direction)
    {
        int maxCells = 3;  // 대시 최대 칸 수
        int cellsMoved = 0;

        // 대시 시에도 방향에 따라 적용
        bool isDiagonal = Mathf.Abs(direction.x) > 0.01f && Mathf.Abs(direction.y) > 0.01f;

        while (cellsMoved < maxCells)
        {
            // 대시 중에도 행동력 부족 시 더 이상 이동하지 않음 (이 부분은 필요에 따라 추가할 수 있음)
            if (_playerStats != null && _playerStats.ActionPoints < MoveActionCost)
            {
                Debug.Log("행동력이 부족하여 대시 중단");
                break;
            }

            Vector3 currentCell = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), transform.position.z);
            Vector3 nextCell = currentCell + new Vector3(Mathf.Sign(direction.x), Mathf.Sign(direction.y), 0);
            Vector3 destination = nextCell + new Vector3(0.5f, 0.5f, 0);

            if (Physics2D.OverlapPoint(destination, ObstacleLayer))
                break;

            yield return StartCoroutine(MoveToTarget(destination, isDiagonal));
            cellsMoved++;
        }
        yield break;
    }
}
