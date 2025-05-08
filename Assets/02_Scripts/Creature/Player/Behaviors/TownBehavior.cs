// TownBehavior.cs

using UnityEngine;
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

        SubscribeInput();
    }

    protected override void SubscribeInput()
    {
        var im = InputManager;
        im.OnMove += HandleMoveInput;
        im.OnDashStart += () => _moveSpeed *= 2f;
        im.OnDashEnd += () => _moveSpeed /= 2f;
        im.OnInteract += HandleInteractInput;
    }

    protected override void UnsubscribeInput()
    {
        var im = InputManager;
        im.OnMove -= HandleMoveInput;
        im.OnDashStart -= () => _moveSpeed *= 2f;
        im.OnDashEnd -= () => _moveSpeed /= 2f;
        im.OnInteract -= HandleInteractInput;
    }

    private void HandleMoveInput(Vector2 raw)
    {
        var dir = raw.normalized;
        if (dir != Vector2.zero)
        {
            if (dir.x != 0f) _spriteRenderer.flipX = dir.x < 0;
            _moveTween?.Kill();

            var target = transform.position + (Vector3)dir * _far;
            _moveTween = transform
                .DOMove(target, _moveSpeed)
                .SetSpeedBased()
                .SetEase(Ease.Linear)
                .OnPlay(() => _animator.PlayMove());
        }
        else
        {
            if (_moveTween != null)
            {
                _moveTween.Kill();
                _moveTween = null;
            }
        }
    }

    private void HandleInteractInput()
    {
        // 필요 시 구현
    }
}
