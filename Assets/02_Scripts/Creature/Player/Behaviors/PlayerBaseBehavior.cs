using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 모든 플레이어 행동(Behavior)의 공통 추상 클래스
/// InputSystem 콜백 메서드를 선언하고, Initialize()에서 바인딩합니다.
/// </summary>
public abstract class PlayerBaseBehavior : MonoBehaviour, PlayerInputActions.IPCActions
{
    protected PlayerController Controller;
    protected PlayerInputActions InputActions;

    /// <summary>
    /// PlayerController 에서 호출하여 초기화
    /// </summary>
    public virtual void Initialize(PlayerController controller)
    {
        Controller = controller;
        InputActions = controller.InputActions;
        InputActions.PC.SetCallbacks(this);
    }

    //  입력 콜백
    public abstract void OnMove(InputAction.CallbackContext ctx);
    public abstract void OnInteract(InputAction.CallbackContext ctx);
    public abstract void OnCancel(InputAction.CallbackContext ctx);
    public abstract void OnMenu(InputAction.CallbackContext ctx);
    public abstract void OnDash(InputAction.CallbackContext ctx);
    public abstract void OnAttackDirection(InputAction.CallbackContext ctx);
    public abstract void OnAttack(InputAction.CallbackContext ctx);
    public abstract void OnMousePosition(InputAction.CallbackContext ctx);
    public abstract void OnMouseClick(InputAction.CallbackContext ctx);
    public abstract void OnCtrl(InputAction.CallbackContext ctx);
}