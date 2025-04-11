using UnityEngine;

public class UI_Menu : UIBase
{
    public override void HideDirect()
    {
        UIManager.Hide<UI_Menu>();
    }

    public override void Opened(params object[] param)
    {
        
    }

    public void OnClickEquipment()
    {
        UIManager.Show<UI_Equipment>();
        HideDirect();
    }

    public void OnClickCraft()
    {
        UIManager.Show<UI_Craft>();
        HideDirect();
    }

    public void OnClickInventory()  // Call At OnClick Event
    {
        UIManager.Show<UI_Inventory>();
        HideDirect();
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
}
