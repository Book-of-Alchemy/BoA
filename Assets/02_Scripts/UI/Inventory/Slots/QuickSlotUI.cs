using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlotUI : SlotUIBase<InventoryItem>, IDropHandler
{
    [SerializeField] private TextMeshProUGUI _indexText;

    public void Initialize(int index)
    {
        Index = index;
        _indexText.text = Index.ToString();
        ClearUI();
    }

    protected override void UpdateUI(InventoryItem data)
    {
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

    public override void OnClick()
    {
        
    }

    public void UseItem()
    {
        if (_data == null) return;

        if (_data.itemData.effect_type is Effect_Type.Damage)
        {
            var controller = GameManager.Instance.PlayerTransform.GetComponent<DungeonBehavior>();
            controller.UseItem(_data.itemData);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var slotUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (slotUI != null && slotUI.HasData)
        {
            UpdateUI(slotUI.Data);
        }
    }

}
