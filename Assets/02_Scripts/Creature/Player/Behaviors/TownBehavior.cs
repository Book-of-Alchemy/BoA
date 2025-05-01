using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(CharacterAnimator), typeof(SpriteRenderer))]
public class TownBehavior : PlayerBaseBehavior
{
    [Header("Town Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    private const float _far = 1000f;

    private CharacterAnimator _animator;
    private SpriteRenderer _spriteRenderer;
    private Tween _moveTween;

    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);
        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 raw = ctx.ReadValue<Vector2>();
        Vector2 dir = raw.normalized;

        if (ctx.performed && dir != Vector2.zero)
        {
            // 좌우 반전 (수평 이동 시)
            if (dir.x != 0f)
                _spriteRenderer.flipX = dir.x < 0;

            // 기존 트윈 있으면 죽이고
            _moveTween?.Kill();

            // 현재 위치 기준 _far 만큼 dir 방향으로 이동하는 트윈
            Vector3 target = transform.position + (Vector3)dir * _far;

            _moveTween = transform
                .DOMove(target, _moveSpeed)    //  두 번째 파라미터는 속도
                .SetSpeedBased()
                .SetEase(Ease.Linear)
                .OnPlay(() => _animator.PlayMove());
        }
        else if (ctx.canceled || dir == Vector2.zero)
        {
            // 키 뗄 때 바로 멈추기
            if (_moveTween != null)
            {
                _moveTween.Kill();
                _moveTween = null;
            }
        }
    }

    public override void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            _moveSpeed *= 2f;
        else if (ctx.canceled)
            _moveSpeed /= 2f;
    }

    public override void OnInteract(InputAction.CallbackContext ctx)
    {
        //if (ctx.started)
    }

    // 사용 안 하는 콜백은 빈 구현
    public override void OnAttack(InputAction.CallbackContext ctx) { }
    public override void OnCancel(InputAction.CallbackContext ctx) { }
    public override void OnMenu(InputAction.CallbackContext ctx) { }
    public override void OnAttackDirection(InputAction.CallbackContext ctx) { }
    public override void OnMousePosition(InputAction.CallbackContext ctx) { }
    public override void OnMouseClick(InputAction.CallbackContext ctx) { }
    public override void OnCtrl(InputAction.CallbackContext ctx) { }
}
