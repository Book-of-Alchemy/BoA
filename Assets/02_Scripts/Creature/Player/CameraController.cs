using UnityEngine;
using Cinemachine;

public class CameraController : Singleton<CameraController>
{
    [Header("이동 설정")]
    [SerializeField] private float panSpeed = 1f;

    [Header("줌 설정")]
    [SerializeField] private float zoomStep = 0.1f;  // 한 번에 늘어나는 단위
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;

    [Header("흔들림 소스")]
    [SerializeField] private CinemachineImpulseSource _source;

    private CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer transposer;
    private Vector2 lastMouseScreenPos;

    // Awake 시 한 번 저장할 초기 offset
    private Vector2 initialOffset;

    // UI가 열려있는지 체크하는 메서드
    private bool IsUIOpen()
    {
        // 필요한 UI 창 타입을 모두 체크
        return UIManager.IsOpened<UI_Menu>()
            || UIManager.IsOpened<UI_Inventory>()
            || UIManager.IsOpened<UI_Setting>()
            || UIManager.IsOpened<UI_Research>()
            || UIManager.IsOpened<UI_SelectQuest>();
    }

    protected override void Awake()
    {
        base.Awake();

        vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer == null)
        {
            enabled = false;
            return;
        }

        // 최초 offset 저장
        initialOffset = transposer.m_TrackedObjectOffset;
    }

    private void OnEnable()
    {
        var im = InputManager.Instance;
        im.EnableMouseTracking = true;
        im.OnRightClickStart += OnPanStart;
        im.OnPan += OnPan;
        im.OnRightClickEnd += OnPanEnd;
        im.OnZoom += OnZoom;
    }

    private void OnDisable()
    {
        var im = InputManager.Instance;
        if (im == null) return;
        im.OnRightClickStart -= OnPanStart;
        im.OnPan -= OnPan;
        im.OnRightClickEnd -= OnPanEnd;
        im.OnZoom -= OnZoom;
        im.EnableMouseTracking = false;
    }

    private void OnPanStart()
    {
        if (IsUIOpen())
            return;
            
        lastMouseScreenPos = InputManager.Instance.MouseScreenPosition;
    }

    private void OnPan(Vector2 screenPos)
    {
        if (IsUIOpen())
            return;
            
        float camZ = -Camera.main.transform.position.z;
        Vector3 lastW = Camera.main.ScreenToWorldPoint(new Vector3(lastMouseScreenPos.x, lastMouseScreenPos.y, camZ));
        Vector3 currentW = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, camZ));

        transposer.m_TrackedObjectOffset += (lastW - currentW) * panSpeed;
        lastMouseScreenPos = screenPos;
    }

    private void OnPanEnd() { /* 필요 시 */ }

    private void OnZoom(float delta)
    {
        if (IsUIOpen())
            return;
            
        var lens = vcam.m_Lens;
        lens.OrthographicSize -= delta * zoomStep;
        vcam.m_Lens = lens;

        CameraZoomLimit();
    }

    /// <summary>
    /// CinemachineVirtualCamera 의 OrthographicSize 를
    /// minZoom/maxZoom 사이로만 유지
    /// </summary>
    private void CameraZoomLimit()
    { 
        var lens = vcam.m_Lens;
        if (lens.OrthographicSize < minZoom) lens.OrthographicSize = minZoom;
        else if (lens.OrthographicSize > maxZoom) lens.OrthographicSize = maxZoom;
        vcam.m_Lens = lens;
    }

    /// <summary>
    /// 이동 완료 시 호출해서 offset 만 복구
    /// </summary>
    public void RestoreCameraState()
    {
        transposer.m_TrackedObjectOffset = initialOffset;
    }
    public void DoImpulse()
    {
        _source.GenerateImpulse();
    }
}
