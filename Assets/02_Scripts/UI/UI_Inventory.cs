using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : UIBase
{
    [SerializeField] private ItemInfo _itemInfo;
    [SerializeField] private Inventory _inventory; //데이터를 가지고 있는 인벤토리
    [SerializeField] private List<InventorySlotUI> _slotUIList; //= new(); //인벤토리 UI가 가지고있는 SlotUI를 리스트로 가지고 있음.

    public override void HideDirect() //Call at OnClick Event 
    {
        UIManager.Hide<UI_Inventory>();
    }

    public override void Opened(params object[] param)
    {
        
    }

    public void SetSlotItem(Sprite sprite, int index, int amount =1) //슬롯에 아이템 UI 갱신
    {
        _slotUIList[index].SetItem(sprite);
        _slotUIList[index].SetItemAmount(amount);
    }

    public void RemoveItem(int index) // 슬롯에 아이템 아이콘, 갯수 제거
    {
        _slotUIList[index].RemoveItem();
    }


    public void OnSlotSelected(InventorySlotUI slot)
    {
        //ItemInfo.Instance.ShowInfo(slot._item.TestItemData);
    }

    public void OnSlotDeselected()
    {
        ItemInfo.Instance.ClearInfo();
    }
}
