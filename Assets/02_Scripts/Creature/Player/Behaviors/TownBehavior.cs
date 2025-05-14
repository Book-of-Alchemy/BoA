using UnityEngine;
using DG.Tweening;
using System.Linq;
public interface IFacilityUI
{
    void ShowUI();
}

[RequireComponent(typeof(CharacterAnimator), typeof(SpriteRenderer))]
public class TownBehavior : PlayerBaseBehavior
{
    [Header("Town Settings")]
    [SerializeField] private float baseMoveSpeed = 5f;
    private Rigidbody2D _rb;
    private CharacterAnimator _animator;
    private SpriteRenderer _spriteRenderer;
    private Tween _moveTween;
    private bool _isDashing = false;
    private FacilitySensor _facilitySensor;
    private InteractZone _interactZone;
    private Vector2 _moveDir;
    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);

        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _interactZone = GetComponentInChildren<InteractZone>();
        _facilitySensor = GetComponentInChildren<FacilitySensor>();
        SubscribeInput();
    }
    private void FixedUpdate()
    {
        if (_moveDir != Vector2.zero)
        {
            float speed = _isDashing ? baseMoveSpeed * 2f : baseMoveSpeed;
            _rb.MovePosition(_rb.position + _moveDir * speed * Time.fixedDeltaTime);
            _animator.SetWalking(true);

            if (Mathf.Abs(_moveDir.x) > 0.01f)
                _spriteRenderer.flipX = _moveDir.x < 0;
        }
        else
        {
            _animator.SetWalking(false);
        }
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
    }

    private void HandleDashEnd()
    {
        _isDashing = false;
    }

    private void HandleMoveInput(Vector2 raw) => _moveDir = raw.normalized;

    private void HandleInteractInput()
    {
        //시설 체크
        var construct = _facilitySensor?.CurrentFacility;
        if (construct != null)
        {
            var uiHandler = construct.GetComponent<IFacilityUI>();
            if (uiHandler != null)
            {
                UnsubscribeInput();
                uiHandler.ShowUI();
                return;
            }
        }

        //Npc체크
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
            //var npc = closest.GetComponent<NPC>();
            //if (npcComp != null)
            //    npc.ShowDialogue();
            //else
               Debug.LogWarning("NPC 컴포넌트가 없습니다.");
        }
    }
    //Shop예시
    //public class ShopFacility : MonoBehaviour, IFacilityUI
    //{
    //    public void ShowUI()
    //    {
    //        // UIManager 쪽에선 이 메서드를 통해 상점 UI를 띄움
    //        UIManager.Instance.OpenShopUI(this);
    //    }
    //}
}

