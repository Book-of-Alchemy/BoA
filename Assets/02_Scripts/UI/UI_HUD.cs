using UnityEngine;

public class UI_HUD : UIBase
{
    public override void HideDirect()
    {

    }

    public override void Opened(params object[] param)
    {

    }

    public void OnClickMenu() // Call At OnClick Event
    {
        UIManager.Show<UI_Inventory>();
    }

}
