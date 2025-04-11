using UnityEngine;

public class UI_Craft : UIBase
{
    public override void HideDirect()
    {
        UIManager.Hide<UI_Craft>();
    }

    public override void Opened(params object[] param)
    {
        
    }
}
