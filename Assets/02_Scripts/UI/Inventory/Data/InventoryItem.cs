using System;
using UnityEngine;

public class InventoryItem : IItemObservable // Inventory에 배열로 존재하는 Item
{
    //아이템의 고유 ID 인덱스와 아이템의id와는 별개로
    public int Amount { get; private set; }
    public ItemData itemData { get; private set; }
    public bool IsEmpty => itemData == null || Amount <= 0;

    public event Action OnItemChanged;

    public void AddItem(ItemData data, int amount = 1)
    {
        //itemData가 비어있다면 아이템 추가
        if (itemData == null)
            itemData = data;

        //아이템 id가 같을때만 수량증가
        if (itemData.id == data.id)
        {
            Amount += amount;
            OnItemChanged?.Invoke();
        }
    }

    public int DecreaseAmount(int amount = 1) //아이템 제거 0이되면 Null
    {
        Amount -= amount;

        if(Amount <= 0)
        {
            Amount = 0;
            itemData = null;
        }
        OnItemChanged?.Invoke();
        return Amount;
    }

    public Sprite GetSprite()
    {
        if(itemData != null)
            return itemData.sprite;
        else
        {
            return null;
        }
    }

    public int GetItemId()
    {
        if (itemData != null)
            return itemData.id;
        else
        {
            return -1;
        }
    }

    public string GetItemName()
    {
        if (itemData != null)
            return itemData.name_kr;
        else
        {
            return string.Empty;
        }
    }

    public string GetItemDesc()
    {
        if (itemData != null)
            return itemData.iteminfo_kr;
        else
        {
            return string.Empty;
        }
    }

    public Item_Type GetItemType() => itemData.item_type;
}
