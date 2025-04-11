using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

[RequireComponent(typeof(CanvasGroup))]
public class SlotItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private TestItem _itemData; //테스트 아이템정보
    public Image _image; // 아이템 Sprite

    public int quantity;
    public bool isEmpty => _itemData == null || quantity == 0;

    [SerializeField] private Canvas canvas; //최상단 캔버스
    [SerializeField] private RectTransform rectTransform; 
    [SerializeField] private CanvasGroup canvasGroup; // S의 CanvasGroup

    //원래의 위치값(알맞은 위치에 드롭하지 못했을 경우 복귀 위치)
    public Vector2 originalPosition;
    public Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalParent = GetComponentInParent<Transform>().parent;
    }

    public void Init(TestItem item)
    {
        _itemData = item;
        _image.sprite = item._Icon;
    }

    public void Clear()
    {
        _itemData = null;
        _image.sprite = null;
    }


    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false; // 슬롯에서 Raycast 허용 (Drop 감지)
        transform.SetParent(canvas.transform); // UI 최상단으로 이동 , 다른 UI 아래에 보이지 않고 위에 보이도록
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        ); //스크린 좌표를 Canvas 내부의 로컬 좌표로 변환
        rectTransform.anchoredPosition = localPoint; // 변환된 좌표로 UI를 이동시켜 드래그 효과를 구현
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; //raycast를 다시 활성화하여 일반적인 클릭/터치 처리가 가능하도록 복원
        // 드롭 실패 시 복귀
        if (transform.parent == canvas.transform) // 드롭이 실패하여 슬롯에 들어가지 못하고 여전히 Canvas 하위에 있을 경우를 감지
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
