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
    private bool _isDashing = false;

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
        im.OnDashStart += HandleDashStart;
        im.OnDashEnd += HandleDashEnd;
        im.OnInteract += HandleInteractInput;
    }

    protected override void UnsubscribeInput()
    {
        var im = InputManager;
        im.OnMove -= HandleMoveInput;
        im.OnDashStart -= HandleDashStart;
        im.OnDashEnd -= HandleDashEnd;
        im.OnInteract -= HandleInteractInput;
    }

    private void HandleDashStart()
    {
        _isDashing = true;
        if (_moveTween != null)
        {
            // 이동 중이면 바로 트윈 속도 두 배
            _moveTween.timeScale = 2f;
        }
        else
        {
            // 아직 이동 전이면 다음 이동 속도를 두 배로
            _moveSpeed *= 2f;
        }
    }

    private void HandleDashEnd()
    {
        _isDashing = false;
        if (_moveTween != null)
        {
            // 이동 중이면 트윈 속도 원상복구
            _moveTween.timeScale = 1f;
        }
        else
        {
            // 아직 이동 전이면 다음 이동 속도 복구
            _moveSpeed /= 2f;
        }
    }

    private void HandleMoveInput(Vector2 raw)
    {
        var dir = raw.normalized;
        if (dir != Vector2.zero)
        {
            if (dir.x != 0f) _spriteRenderer.flipX = dir.x < 0;

            // 이전 트윈이 있으면 죽이고 새로 생성
            _moveTween?.Kill();

            var target = transform.position + (Vector3)dir * _far;
            _moveTween = transform
                .DOMove(target, _moveSpeed)
                .SetSpeedBased()
                .SetEase(Ease.Linear)
                .OnPlay(() => _animator.SetWalking(true));

            // 대시 중이었다면 바로 속도 적용
            if (_isDashing)
                _moveTween.timeScale = 2f;
        }
        else
        {
            // 입력이 0이 되면 이동 중지
            if (_moveTween != null)
            {
                _moveTween.Kill();
                _moveTween = null;
                _animator.SetWalking(false);
            }
        }
    }

    private void HandleInteractInput()
    {
        // 필요 시 구현
    }
}
