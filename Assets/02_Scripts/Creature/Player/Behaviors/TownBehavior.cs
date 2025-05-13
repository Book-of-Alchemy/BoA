using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core;        // Physics2D 모듈 네임스페이스
using DG.Tweening.Plugins.Options;
using System.Linq;

[RequireComponent(typeof(CharacterAnimator), typeof(SpriteRenderer))]
public class TownBehavior : PlayerBaseBehavior
{
    [Header("Town Settings")]
    [SerializeField] private float baseMoveSpeed = 5f;
    private const float _far = 1000f;
    private Rigidbody2D _rb;
    private CharacterAnimator _animator;
    private SpriteRenderer _spriteRenderer;
    private Tween _moveTween;
    private bool _isDashing = false;
    private Collider2D _interactZoneCollider;
    private InteractZone _interactZone;
    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);

        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _interactZoneCollider = GetComponentInChildren<Collider2D>();
        _interactZone = GetComponentInChildren<InteractZone>();
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
            // 이미 이동 중일 때만 timeScale로 속도 두 배
            _moveTween.timeScale = 2f;
        }
    }

    private void HandleDashEnd()
    {
        _isDashing = false;
        if (_moveTween != null)
        {
            // 이동 종료 후에는 timeScale 원상복구
            _moveTween.timeScale = 1f;
        }
    }

    private void HandleMoveInput(Vector2 raw)
    {
        Vector2 dir = raw.normalized;
        if (dir != Vector2.zero)
        {
            CameraController.Instance.RestoreCameraState();
            // 좌우 플립 처리
            if (dir.x != 0f)
                _spriteRenderer.flipX = dir.x < 0;

            // 기존 트윈 종료
            _moveTween?.Kill();

            Vector2 target = _rb.position + dir * _far;
            _moveTween = _rb
                .DOMove(target, baseMoveSpeed)
                .SetSpeedBased()
                .SetEase(Ease.Linear)
                .SetUpdate(UpdateType.Fixed)
                .OnPlay(() => _animator.SetWalking(true))
                .OnKill(() => _animator.SetWalking(false));

            // 대시 상태라면 timeScale 적용
            if (_isDashing)
                _moveTween.timeScale = 2f;
        }
        else
        {
            // 입력이 없으면 이동 및 애니메이션 종료
            _moveTween?.Kill();
            _moveTween = null;
        }
    }

    private void HandleInteractInput()
    {
        if (_interactZone == null)
            return;

        var npcs = _interactZone.NPCs;
        if (npcs.Count == 0)
        {
            Debug.Log("주변에 NPC가 없습니다.");
            return;
        }

        var closest = npcs
            .OrderBy(c => (c.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault();

        if (closest != null)
        {
            //var npcComp = closest.GetComponent<NPC>();
            //if (npcComp != null)
            //    npcComp.ShowDialogue();
            //else
               Debug.LogWarning("NPC 컴포넌트가 없습니다.");
        }
    }
}

