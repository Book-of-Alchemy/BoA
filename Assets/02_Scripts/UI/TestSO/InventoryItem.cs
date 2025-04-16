using UnityEngine;

public class InventoryItem // Inventory에 배열로 존재하는 Item
{
    private int _amount;
    public InventoryItemData InventorItemData { get; private set; }

    public void AddItem(InventoryItemData data)
    {
        InventorItemData = data;
    }

    public void AddAmount(int amount)
    {
        _amount += amount;
    }
}
