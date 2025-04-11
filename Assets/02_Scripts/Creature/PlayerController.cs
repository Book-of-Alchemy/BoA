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

    public float MoveSpeed = 5f;    // 이동 속도
    public float DashSpeed = 10f;   // 대시 속도 (참고용)
    private bool _isMoving = false;
    private Vector3 _targetPosition;

    public LayerMask ObstacleLayer; // 대시 중 장애물(적, 함정, 이동 불가 타일 등) 체크용

    // 마지막 이동 방향 (대시 시 사용)
    private Vector2 _lastMoveDirection = Vector2.down;

    void Awake()
    {
        // 이동 액션 (WASD, 2D 벡터 조합)
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

    void Update()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        if (input != Vector2.zero && !_isMoving)
        {
            // 마지막 이동 방향 갱신
            _lastMoveDirection = input;

            // 현재 pos를 그리드 좌표로 변환 후 이동 벡터 더함
            Vector3 currentGrid = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), transform.position.z);
            Vector3 moveVector = new Vector3(input.x, input.y, 0);
            Vector3 newGridPos = currentGrid + moveVector;

            // 그리드 셀 중앙(오프셋 0.5)으로 목표 pos 설정
            _targetPosition = new Vector3(newGridPos.x + 0.5f, newGridPos.y + 0.5f, transform.position.z);

            // 목표 pos로 이동 코루틴 호출
            StartCoroutine(MoveToTarget(_targetPosition));
        }

        if (_confirmAction.triggered)
            Debug.Log("상호작용"); // 상호작용, 아이템 사용, 대화, 메뉴 진입 등 처리

        if (_cancelAction.triggered)
            Debug.Log("취소 누름"); // 선택 취소, 메뉴 닫기, 대화 스킵 등 처리

        if (_menuAction.triggered)
            Debug.Log("메뉴누름"); // 인벤토리/제작 등 주요 메뉴 호출 처리

        if (_dashAction.triggered)
        {
            Debug.Log("대쉬 활성화");
            StartCoroutine(Dash(_lastMoveDirection));
        }
    }

    // 목표 pos까지 부드럽게 이동하는 코루틴
    IEnumerator MoveToTarget(Vector3 destination)
    {
        _isMoving = true;
        while ((destination - transform.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, MoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destination;
        _isMoving = false;
        yield break;
    }

    // 대시 코루틴: 최대 지정 칸만큼 이동, 장애물 감지 시 중지
    IEnumerator Dash(Vector2 direction)
    {
        int maxCells = 3;  // 대시 최대 칸 수
        int cellsMoved = 0;

        while (cellsMoved < maxCells)
        {
            Vector3 currentGrid = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), transform.position.z);
            Vector3 nextGrid = currentGrid + new Vector3(direction.x, direction.y, 0);
            Vector3 destination = new Vector3(nextGrid.x + 0.5f, nextGrid.y + 0.5f, transform.position.z);

            // 목적지가 장애물인 경우 대시 중지
            if (Physics2D.OverlapPoint(destination, ObstacleLayer))
                break;

            yield return StartCoroutine(MoveToTarget(destination));
            cellsMoved++;
        }
        yield break;
    }
}
