using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;
    [SerializeField] private GameObject dragIconPrefab; //드래그시 생성될 프리팹
    private GameObject _dragIcon;
    private InventoryItem _draggedItem;
    private IDraggableSlot _originSlot;
    public bool IsDragging => _draggedItem != null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void BeginDrag(IDraggableSlot originSlot, InventoryItem item, Sprite icon, Vector3 startPosition)
    {
        if (IsDragging) return;

        _originSlot = originSlot;
        _draggedItem = item;

        //드래그용 복제 아이콘생성
        _dragIcon = Instantiate(dragIconPrefab, transform);
        //마우스가 클릭된 위치반환 후 시작 위치로 저장
        _dragIcon.transform.position = startPosition;

        var image = _dragIcon.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = icon;
            image.raycastTarget = false;
        }
    }

    public void UpdateDrag(Vector3 position)
    {
        if (_dragIcon != null)
            _dragIcon.transform.position = position;
    }

    public void EndDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
            Destroy(_dragIcon);

        if (_draggedItem == null)
        {
            ResetState();
            return;
        }

        if (eventData.pointerEnter != null)
        {
            var dropTarget = eventData.pointerEnter.GetComponentInParent<IDraggableSlot>();

            //CraftSlotUI는 드래그 대상에서 제외
            if (dropTarget is CraftSlotUI)
            {
                ResetState(); // 아무 것도 하지 않음
                return;
            }

            if (_originSlot is InventorySlotUI originInvSlot &&
                dropTarget is InventorySlotUI targetInvSlot &&
                dropTarget != _originSlot)
            {
                Inventory.Instance.SetItemAtoB(originInvSlot.Index, targetInvSlot.Index);
            }
        }

        ResetState();
    }
    private void ResetState()
    {
        _draggedItem = null;

        if (_dragIcon != null)
            Destroy(_dragIcon);

        _dragIcon = null;
    }
}