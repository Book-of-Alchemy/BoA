using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterAnimator))]
public class TownBehavior : PlayerBaseBehavior
{
    [Header("Town Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private CharacterAnimator animator;
    private Coroutine moveCoroutine;

    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);
        animator = GetComponent<CharacterAnimator>();
    }

    public override void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();
        if (ctx.performed && dir != Vector2.zero)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveRoutine(dir));
        }
        else if (ctx.canceled || dir == Vector2.zero)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    private IEnumerator MoveRoutine(Vector2 dir)
    {
        while (true)
        {
            animator.PlayMove();
            transform.Translate((Vector3)dir * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public override void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            moveSpeed *= 2f;
        else if (ctx.canceled)
            moveSpeed /= 2f;
    }

    public override void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            Controller.onActionConfirmed?.Invoke();
    }

    public override void OnCancel(InputAction.CallbackContext ctx) { }
    public override void OnMenu(InputAction.CallbackContext ctx) { }
    public override void OnAttackDirection(InputAction.CallbackContext ctx) { }
    public override void OnAttack(InputAction.CallbackContext ctx) { }
    public override void OnMousePosition(InputAction.CallbackContext ctx) { }
    public override void OnMouseClick(InputAction.CallbackContext ctx) { }
    public override void OnCtrl(InputAction.CallbackContext ctx) { }
}