using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlotUI : MonoBehaviour , IDropHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _indexText;
    [SerializeField] private TextMeshProUGUI _amountText;

    private InventoryItem _item;
    private int _index;

    public void Initialize(int index)
    {
        _index = index;
        _indexText.text = _index.ToString();
        ClearSlot();
    }

    public void SetItem(InventoryItem item)
    {
        _item = item;
        _icon.sprite = item.itemData.Sprite;
        _amountText.text = item.Amount.ToString();
        _icon.enabled = true;
    }

    public void ClearSlot()
    {
        _item = null;
        _amountText.text = string.Empty;
        _icon.enabled = false;
    }

    public void UseItem()
    {
        if (_item == null) return;

        if (_item.itemData.effect_type is Effect_Type.Damage)
        {
            BaseItem baseItem = ItemManager.Instance.CreateItem(_item.itemData);
            baseItem.UseItem(_item.itemData);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var slotUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (slotUI != null && slotUI.HasItem)
        {
            SetItem(slotUI.Item);
        }
    }
}
