using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : UIBase
{
    [SerializeField] private ItemInfo _itemInfo;
    [SerializeField] private Inventory _inventory; //데이터를 가지고 있는 인벤토리
    [SerializeField] private List<InventorySlotUI> _slotUIList; //= new(); //인벤토리 UI가 가지고있는 SlotUI를 리스트로 가지고 있음.

    private void Start()
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            var slot = _slotUIList[i];
            slot.Index = i;

            slot.OnSelected += OnSlotSelected;
            slot.OnDeselected += OnSlotDeselected;
        }
    }

    public override void HideDirect() //Call at OnClick Event 
    {
        UIManager.Hide<UI_Inventory>();
        UIManager.Hide<UI_Action>();
    }

    public override void Opened(params object[] param)
    {
        
    }

    public void SetSlotItem(int index, InventoryItem item) //슬롯에 아이템 UI 갱신
    {
        _slotUIList[index].SetItem(item);
    }

    public void RemoveItem(int index) // 슬롯에 아이템 아이콘, 갯수 제거
    {
        _slotUIList[index].RemoveItem();
    }


    public void OnSlotSelected(int index)
    {
        if (_inventory.items[index] != null)
            _itemInfo.ShowInfo(_inventory.items[index]);
    }

    public void OnSlotDeselected(int index)
    {
        _itemInfo.ClearInfo();
    }
}
