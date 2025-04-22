using UnityEngine;

public class UI_Menu : UIBase
{
    private UI_Inventory _Inventory;

    public override void HideDirect()
    {
        UIManager.Hide<UI_Menu>();
    }

    public override void Opened(params object[] param)
    {
        _Inventory = UIManager.Get<UI_Inventory>(); //인벤토리 캐싱
    }

    public void OnClickEquipment()  // Call At OnClick Event
    {
        if(_Inventory != null)
        {
            //열려있지만 Equipment가 아닌경우 EquipmentTool로 전환
            if (CheckInventoryType(EInventoryType.Equipment))
                return;
        }
        else
            ShowInventory(EInventoryType.Equipment);//열려있지 않은경우
    }

    public void OnClickCraft()  // Call At OnClick Event
    {
        if (_Inventory != null)
        {
            if (CheckInventoryType(EInventoryType.Craft))
                return;
        }
        else
            ShowInventory(EInventoryType.Craft);
    }

    public void OnClickInventory()  // Call At OnClick Event
    {
        if (_Inventory != null)
        {
            if (CheckInventoryType(EInventoryType.Inventory))
                return;
        }
        else
            ShowInventory(EInventoryType.Inventory);
    }

    public void OnClickSetting() // Call At OnClick Event
    {
        UIManager.Show<UI_Setting>();
        HideDirect();
    }

    public void OnClickMainMenu() // Call At OnClick Event
    {
        UIManager.Show<UI_Main>();
        HideDirect();
    }

    private void ShowInventory(EInventoryType inventoryType)// 인벤토리 타입에 따라 Show
    {
        UIManager.Show<UI_Inventory>(inventoryType);
        HideDirect();
    }

    private bool CheckInventoryType(EInventoryType inventoryType) 
    {
        if (_Inventory.curType != inventoryType)
        {
            _Inventory.ShowRightTool(inventoryType);
            HideDirect();
            return true;
        }
        else
            return false;
    }
}
