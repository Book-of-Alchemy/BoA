using UnityEngine;

public class CraftSlotUI : InventorySlotUI
{
    public override void OnClick()
    {
        if (!HasData) return;
        Inventory.Instance.RemoveCraftTable(_data);
        ClearUI();
    }

}
