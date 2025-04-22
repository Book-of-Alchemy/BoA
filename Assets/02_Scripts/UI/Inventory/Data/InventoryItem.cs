using UnityEngine;

public class InventoryItem // Inventory에 배열로 존재하는 Item
{
    public int Amount { get; private set; }

    public InventoryItemData InventorItemData { get; private set; }
    public ItemData itemData { get; private set; }

    public void AddItem(InventoryItemData data, int amount = 1)
    {
        if(InventorItemData == null)
            InventorItemData = data;
        Amount += amount;
    }

    public void AddBaseItem(ItemData data, int amount = 1)
    {
        if (itemData == null)
            itemData = data;
        Amount += amount;
        Debug.Log(Amount);
    }

    public int GetReuceAmount() //아이템 제거 0이되면 Null
    {
        Amount--;
        if(Amount == 0)
            itemData = null;
        Debug.Log(Amount);
        return Amount;  
    }

    public Sprite GetSprite()
    {
        //return InventorItemData.Icon;
        return itemData.Sprite;
    }

    public int GetItemId()
    {
        return itemData.id;
    }

    public string GetItemName()
    {
        return itemData.name_kr;
    }

    public string GetItemDesc()
    {
        return itemData.iteminfo_kr;
    }
}
