using UnityEngine;

public class CraftSlotUI : InventorySlotUI
{
    public override void OnClickItem()
    {
        RemoveItem();
    }

}
