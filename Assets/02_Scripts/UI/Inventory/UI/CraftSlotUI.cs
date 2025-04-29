using UnityEngine;

public class CraftSlotUI : InventorySlotUI
{
    public override void OnClickItem()
    {
        if (!HasItem) return;
        Inventory.Instance.RemoveCraftTable(_item);
        RemoveItem();
    }

}
