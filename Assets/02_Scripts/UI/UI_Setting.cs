using UnityEngine;

public class UI_Setting : UIBase
{
    public override void HideDirect()//Call at OnClick Event 
    {
        UIManager.Hide<UI_Setting>();   
    }

    public override void Opened(params object[] param)
    {
        
    }

}
