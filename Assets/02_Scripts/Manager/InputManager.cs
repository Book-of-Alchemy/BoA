using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
    public PlayerInputActions _input;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MouseScreenPosition { get; private set; }
    public Vector3 MouseWorldPosition { get; private set; }

    // 새로 추가된 이벤트들
    public event Action<Vector2> OnMove;          // 방향 입력(perform/cancel)
    public event Action OnAttack;                 // 공격
    public event Action OnInteract;               // 상호작용
    public event Action OnDashStart;              // 대시 시작
    public event Action OnDashEnd;                // 대시 종료
    public event Action OnCtrlStart;              // Ctrl(하이라이트) 누름
    public event Action OnCtrlEnd;                // Ctrl(하이라이트) 뗌
    public event Action<Vector3> OnMouseClick;    // 마우스 클릭
    public event Action<Vector3> OnMouseMove;     // 마우스 이동
    public event Action OnCancel;
    public event Action OnMenu;
    public event Action OnRest;

    public bool EnableMouseTracking { get; set; } = false;

    private Camera _mainCam;

    protected override void Awake()
    {
        base.Awake();
        if (_input == null) _input = new PlayerInputActions();
        _mainCam = Camera.main;

        // 이동
        _input.PC.Move.performed += ctx =>
        {
            MoveInput = ctx.ReadValue<Vector2>();
            OnMove?.Invoke(MoveInput);
        };
        _input.PC.Move.canceled += _ =>
        {
            MoveInput = Vector2.zero;
            OnMove?.Invoke(Vector2.zero);
        };

        // 공격 · 상호작용
        _input.PC.Attack.performed += _ => OnAttack?.Invoke();
        _input.PC.Interact.performed += _ => OnInteract?.Invoke();

        // 대시
        _input.PC.Dash.started += _ => OnDashStart?.Invoke();
        _input.PC.Dash.canceled += _ => OnDashEnd?.Invoke();

        // Ctrl (하이라이트)
        _input.PC.Ctrl.started += _ => OnCtrlStart?.Invoke();
        _input.PC.Ctrl.canceled += _ => OnCtrlEnd?.Invoke();

        // 마우스 이동 · 클릭
        _input.PC.MousePosition.performed += ctx =>
        {
            if (!EnableMouseTracking) return;
            MouseScreenPosition = ctx.ReadValue<Vector2>();
            MouseWorldPosition = _mainCam.ScreenToWorldPoint(MouseScreenPosition);
            OnMouseMove?.Invoke(MouseWorldPosition);
        };
        _input.PC.MouseClick.performed += _ =>
        {
            if (!EnableMouseTracking) return;
            // UI 위면 무시
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;
            OnMouseClick?.Invoke(MouseWorldPosition);
        };
        // 취소
        _input.PC.Cancel.performed += _ => OnCancel?.Invoke();
        // 메뉴
        _input.PC.Menu.performed += _ => OnMenu?.Invoke();

        //한턴 쉬기
        _input.PC.Space.performed += _ => OnRest?.Invoke();
    }

    public void OnEnable() => _input.PC.Enable();
    public void OnDisable() => _input.PC.Disable();
}
