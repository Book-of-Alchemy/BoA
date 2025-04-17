using UnityEngine;

public class InventoryItem // Inventory에 배열로 존재하는 Item
{
    public int Amount { get; private set; }

    public InventoryItemData InventorItemData { get; private set; }

    public void AddItem(InventoryItemData data, int amount = 1)
    {
        if(InventorItemData == null)
            InventorItemData = data;
        Amount += amount;
    }

    public Sprite GetSprite()
    {
        return InventorItemData.Icon;
    }

    public int GetItemId()
    {
        return InventorItemData.Id;
    }
}
