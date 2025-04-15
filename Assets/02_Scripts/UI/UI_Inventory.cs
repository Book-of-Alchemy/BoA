using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class UI_Inventory : UIBase
{
    public static UI_Inventory Instance { get; private set; }

    public List<TestItem> randomItem; // AddItem에서 적용할 랜덤한 아이템 SO

    public List<Slots> slots;

    [SerializeField] private ItemInfo _itemInfo;

    private void Awake()
    {
        Instance = this;

    }

    void UpdateIntemInfo()
    {

    }

    //public bool AddItem(TestItem item, int amount)
    //{
    //    foreach (var slot in inventoryData.slots)
    //    {
    //        //중복 또는 최대 수량 도달 안했을때 해당 인벤토리에 추가
    //        if (!slot.IsEmpty && slot.item == item && slot.quantity < item.maxStack)
    //        {
    //            slot.quantity += amount;
    //            return true;
    //        }
    //    }

    //    foreach (var slot in inventoryData.slots)
    //    {
    //        if (slot.IsEmpty)
    //        {
    //            slot.item = item;
    //            slot.quantity = amount;
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //public void RemoveItem(int slotIndex)
    //{
    //    if (slotIndex >= 0 && slotIndex < inventoryData.slots.Count)
    //    {
    //        inventoryData.slots[slotIndex].Clear();
    //    }
    //}

    public override void HideDirect() //Call at OnClick Event 
    {
        UIManager.Hide<UI_Inventory>();
    }

    public override void Opened(params object[] param)
    {
        
    }

    public void OnClickAddItem() //Call at OnClick Event 
    {
        int i = Random.Range(0, randomItem.Count);
        foreach (var slot in slots)
        {
            if(slot._item.TestItemData == null)
            {
                slot.UpdateItem(randomItem[i]);
                break;
            }
        }
        //foreach (var slot in inventoryData.slots)
        //{
        //    //중복 또는 최대 수량 도달 안했을때 해당 인벤토리에 추가
        //    if (!slot.IsEmpty && slot.item == item && slot.quantity < item.maxStack)
        //    {
        //        slot.quantity += amount;
        //        return true;
        //    }
        //}

        //foreach (var slot in inventoryData.slots)
        //{
        //    if (slot.IsEmpty)
        //    {
        //        slot.item = item;
        //        slot.quantity = amount;
        //        return true;
        //    }
        //}
    }

    public void OnSlotSelected(Slots slot)
    {
        
        ItemInfo.Instance.ShowInfo(slot._item.TestItemData);
       
    }

    public void OnSlotDeselected()
    {
        ItemInfo.Instance.ClearInfo();
    }
}
