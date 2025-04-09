using UnityEngine;

public class UI_Inventory : UIBase
{

    public override void HideDirect() //Call at OnClick Event 
    {
        UIManager.Hide<UI_Inventory>();
    }

    public override void Opened(params object[] param)
    {
        
    }
}
