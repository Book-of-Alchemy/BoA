using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : Singleton<InputManager>
{

    public PlayerInputActions _input;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MouseScreenPosition { get; private set; }
    public Vector3 MouseWorldPosition { get; private set; }

    public event Action OnAttack;
    public event Action OnInteract;

    /// <summary>
    /// 필독!  아이템 사용 시작시 마우스 무브와 클릭에 구독 사용이 끝날때 구독해제하면 됨
    /// vector3 매개변수를 받는 void를 구독할것 EnableMouseTracking를 true해준뒤 사용할것
    /// </summary>
    public event Action<Vector3> OnMouseClick;
    public event Action<Vector3> OnMouseMove;

    public bool EnableMouseTracking { get; set; } = false;

    private Camera _mainCam;

    protected override void Awake()
    {
        base.Awake();

        _input = new PlayerInputActions();
        _mainCam = Camera.main;

        _input.PC.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _input.PC.Move.canceled += _ => MoveInput = Vector2.zero;//_사용시 매개변수를 사용하지 않는 경우 ctx는 callback 매개변수를 받아야함

        _input.PC.Attack.performed += _ => OnAttack?.Invoke();
        _input.PC.Interact.performed += _ => OnInteract?.Invoke();

        _input.PC.MousePosition.performed += ctx =>
        {
            if (!EnableMouseTracking) return;
            MouseScreenPosition = ctx.ReadValue<Vector2>();
            MouseWorldPosition = _mainCam.ScreenToWorldPoint(MouseScreenPosition);
            OnMouseMove?.Invoke(MouseWorldPosition);//vector3 매개변수로 받는 메서드로 추가
        };

        _input.PC.MouseClick.performed += _ =>
        {
            if (!EnableMouseTracking) return;
            OnMouseClick?.Invoke(MouseWorldPosition);//vector3 매개변수로 받는 메서드로 추가
        };
    }

    private void OnEnable() => _input.PC.Enable();
    private void OnDisable() => _input.PC.Disable();
}
