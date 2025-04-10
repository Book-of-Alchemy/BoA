using UnityEngine;

public class UI_HUD : UIBase
{
    public override void HideDirect()
    {
        UIManager.Hide<UI_HUD>();
    }

    public override void Opened(params object[] param)
    {

    }

    public void OnClickCraft() // Call At OnClick Event
    {
        UIManager.Show<UI_Craft>();
    }

    public void OnClickMenu() // Call At OnClick Event
    {
        UIManager.Show<UI_Menu>();
    }

}
