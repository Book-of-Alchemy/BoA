using UnityEngine;
using UnityEngine.EventSystems;

public class DragableUI : MonoBehaviour , IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] [Tooltip("드래그 될 UI")]
    private Transform _dragUI;
    [SerializeField] [Tooltip("나가면 원위치 시키는 영역")]
    private RectTransform _dropArea;
    
    private Vector2 _beginPoint; //드래그 시작전 UI위치
    private Vector2 _moveBegin; // 드래그 시작전 마우스(터치)위치

    private void Awake()
    {
        // 드래그 될 UI를 지정하지 않았다면, 부모로 초기화
        if (_dragUI == null)
            _dragUI = transform.parent;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) //마우스,터치 입력을 감지 
    {
        _beginPoint = _dragUI.position;
        _moveBegin = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData) //Drag중 계속 호출
    {
        //드래그 전 위치 대비 현재 입력 위치의 차이를 구해 그만큼 이동
        _dragUI.position = _beginPoint + (eventData.position - _moveBegin);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)// 터치 입력을 떼는 경우
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(_dropArea, eventData.position)) // dropArea 내인지 확인
        {
            _dragUI.position = _beginPoint; // 드래그 전 위치로 되돌림
        }
    }
}
