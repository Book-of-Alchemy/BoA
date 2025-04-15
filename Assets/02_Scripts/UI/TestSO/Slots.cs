using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class Slots : MonoBehaviour, IDropHandler
{
    [SerializeField] private SlotSelect _btn;
    public SlotItem _item;
    [SerializeField] private TextMeshProUGUI _numText;

    public void UpdateItem(TestItem item)
    {
        _item.UpdateSlotItem(item);
    }

    public eTestItemType itemType; //테스트
    public bool allowAnyType = true;

    void IDropHandler.OnDrop(PointerEventData eventData) // Drag멈출때 호출되는 함수
    {
        GameObject droppedObject = eventData.pointerDrag; // 드래그 중인 GameObject를 가져온다.

        //if (droppedObject != null)
        if (droppedObject.TryGetComponent(out SlotItem item)) // 수정 예정
        {
            RectTransform droppedTransform = item.GetComponent<RectTransform>(); // 드래그 된 오브젝트 위치 참조
            //SlotItem item = droppedObject.GetComponent<SlotItem>(); // 타입 변환

            if (transform.childCount > 1 && _item.isActiveAndEnabled) //현재 Slot하위에 btn이 있기때문에 1
            {
                // 슬롯에 이미 아이템이 있다면 교체 또는 스왑
                Transform existingItem = transform.GetComponentInChildren<SlotItem>().transform;
                existingItem.SetParent(item.originalParent); // 원래 자리로 이동
                existingItem.GetComponent<RectTransform>().anchoredPosition = item.originalPosition;
            }

            // 드래그된 아이템을 현재 슬롯으로 이동
            droppedObject.transform.SetParent(transform);
            droppedTransform.anchoredPosition = Vector2.zero;
        }
    }
}
