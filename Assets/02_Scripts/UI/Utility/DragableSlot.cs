using UnityEngine;
using UnityEngine.EventSystems;
public interface IDraggableSlot
{
    int Index { get; }
    bool HasData { get; }
    InventoryItem GetItem(); // 드래그 시 참조할 데이터
}

[RequireComponent(typeof(RectTransform))]
public class DragableSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private IDraggableSlot _slot;
    private Vector2 _pointerDownPos;
    private const float DragThreshold = 10f; //드래그 유효범위

    private void Awake()
    {
        _slot = GetComponent<IDraggableSlot>();
        if (_slot == null)
            Debug.LogError("IDraggableSlot 인터페이스를 구현한 컴포넌트가 필요합니다.");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_slot.HasData) return;
        _pointerDownPos = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_slot == null || !_slot.HasData)
            return;

        if (Vector2.Distance(eventData.position, _pointerDownPos) < DragThreshold)
            return;

        var icon = _slot.GetItem().GetSprite();
        Debug.Log($"드래그 된 아이템 정보 : {_slot.GetItem().GetItemName()}");
        DragManager.Instance.BeginDrag(_slot, _slot.GetItem(), icon, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragManager.Instance.IsDragging)
            DragManager.Instance.UpdateDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragManager.Instance.EndDrag(eventData);
    }
}