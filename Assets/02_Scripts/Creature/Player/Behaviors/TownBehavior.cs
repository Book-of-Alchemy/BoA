using UnityEngine;
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
    
    private FacilitySensor _facilitySensor;
    private InteractZone _interactZone;
    
    private Vector2 _moveDir;
    private bool _isDashing = false;

    private Vector2 _clickTarget;
    private bool _isAutoMoving = false;
    private float _stopPoint = 0.1f;

    private bool IsUIOpen()
    {
        return UIManager.IsOpened<UI_Research>()
            || UIManager.IsOpened<UI_SelectQuest>();
    }

    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);

        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _interactZone = GetComponentInChildren<InteractZone>();
        _facilitySensor = GetComponentInChildren<FacilitySensor>();
        InputManager.Instance.EnableMouseTracking = true;
        SubscribeInput();
    }
    private void FixedUpdate()
    {
        // 자동 이동 로직
        if (_isAutoMoving)
        {
            Vector2 currentPos = _rb.position;
            Vector2 toTarget = _clickTarget - currentPos;
            if (toTarget.magnitude <= _stopPoint)
            {
                _isAutoMoving = false;
                _moveDir = Vector2.zero;
                _animator.SetWalking(false);
            }
            else
            {
                _moveDir = toTarget.normalized;
            }
        }

        if (_moveDir != Vector2.zero)
        {
            float speed = _isDashing ? baseMoveSpeed * 2f : baseMoveSpeed;
            _rb.MovePosition(_rb.position + _moveDir * speed * Time.fixedDeltaTime);
            _animator.SetWalking(true);

            if (Mathf.Abs(_moveDir.x) > 0.01f)
                _spriteRenderer.flipX = _moveDir.x < 0;

            if(Mathf.Abs(_moveDir.x)>0.01f|| Mathf.Abs(_moveDir.y) > 0.01f)
                CameraController.Instance.RestoreCameraState();
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
        im.OnMouseClick += HandleMouseClick;
        im.OnCancel += HandleCancelInput;
        im.OnMenu += HandleOnMenuInput;
    }

    protected override void UnsubscribeInput()
    {
        var im = InputManager;
        im.OnMove -= HandleMoveInput;
        im.OnDashStart -= HandleDashStart;
        im.OnDashEnd -= HandleDashEnd;
        im.OnInteract -= HandleInteractInput;
        im.OnMouseClick -= HandleMouseClick;
        im.OnCancel -= HandleCancelInput;
        im.OnMenu -= HandleOnMenuInput;
    }

    private void HandleCancelInput()
    {
        UIManager.CloseLastOpenedUI();
    }

    private void HandleDashStart()
    {
        if (IsUIOpen())
            return;
            
        _isDashing = true;
    }

    private void HandleDashEnd()
    {
        _isDashing = false;
    }

    private void HandleMoveInput(Vector2 raw)
    {
        if (IsUIOpen())
            return;
            
        _isAutoMoving = false;
        _moveDir = raw.normalized;
    }

    private void HandleMouseClick(Vector3 worldPos)
    {
        if (IsUIOpen())
            return;
            
        _clickTarget = new Vector2(worldPos.x, worldPos.y);
        _isAutoMoving = true;
    }

    private void HandleInteractInput()
    {
        if (IsUIOpen())
            return;
            
        //시설 체크
        var construct = _facilitySensor?.CurrentFacility;
        if (construct != null)
        {
            var uiHandler = construct.GetComponent<IFacilityUI>();
            if (uiHandler != null)
            {
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
        }
    }
    private void HandleOnMenuInput()
    {
        UIManager.ShowOnce<UI_Text>("메뉴 구현 전");
    }
}

