using UnityEngine;

public class InventoryItem // Inventory에 배열로 존재하는 Item
{
    public int Amount { get; private set; }
    public ItemData itemData { get; private set; }
    public void AddItem(ItemData data, int amount = 1)
    {
        Amount += amount;
        if (itemData == null)
            itemData = data;
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
    public Item_Type GetItemType()
    {
        return itemData.item_type;
    }
}
