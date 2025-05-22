using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlotUI : SlotUIBase<InventoryItem>, IDropHandler, IDraggableSlot
{
    [SerializeField] private TextMeshProUGUI _indexText;
    public InventoryItem GetItem() => _data;
    public void Initialize(int index)
    {
        Index = index;
        _indexText.text = Index.ToString();
        ClearUI();
    }

    protected override void UpdateUI(InventoryItem data)
    {
        if (_icon == null || data?.itemData == null)
            return;
        _data = data;
        _icon.sprite = data.itemData.sprite;
        _countTxt.text = data.Amount.ToString();
        _icon.enabled = true;
    }

    protected override void ClearUI()
    {
        _data = null;
        _countTxt.text = string.Empty;
        _icon.enabled = false;
    }
    protected override void ShowTooltip(InventoryItem data)
    {
        if (data == null || data.itemData == null) return;

        UIManager.Show<UI_ItemTooltip>(data.itemData);
    }

    protected override void HideTooltip()
    {
        UIManager.Hide<UI_ItemTooltip>();
    }
    public override void OnClick()
    {
        
    }

    public void UseItem()
    {
        if (_data == null || _data.itemData == null)
        {
            ClearUI();
            return;
        }

        int index = Inventory.Instance.GetItemIndex(_data.itemData.id);

        if (index >= 0)
        {
            Inventory.Instance.Use(_data, index);
        }
        else
        {
            Debug.LogWarning("아이템이 인벤토리에 존재하지 않음");
            ClearUI();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var originSlot = eventData.pointerDrag?.GetComponent<IDraggableSlot>();
        if (originSlot == null || originSlot.GetItem() == null) return;

        var draggedItem = originSlot.GetItem();

        // Consumable 타입만 허용
        if (draggedItem.itemData.item_type != Item_Type.Consumable) return;

        // 같은 item_id가 다른 슬롯에 존재하면 제거
        var quickSlotParent = GetComponentInParent<QuickSlot>();
        if (quickSlotParent != null)
        {
            foreach (var slot in quickSlotParent.Slots)
            {
                if (slot == this) continue;

                if (slot.HasData && slot.GetItem().itemData.id == draggedItem.itemData.id)
                {
                    slot.ClearUI();
                    break;
                }
            }
        }

        // 슬롯 간 위치 교환
        if (originSlot is QuickSlotUI originQuickSlot)
        {
            var temp = this._data;
            this.UpdateUI(originQuickSlot.GetItem());
            originQuickSlot.UpdateUI(temp);
        }
        else
        {
            // 인벤토리에서 퀵슬롯으로 등록
            this.UpdateUI(draggedItem);
        }
        SetData(draggedItem);
    }
}
