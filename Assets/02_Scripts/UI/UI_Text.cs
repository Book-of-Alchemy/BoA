using TMPro;
using UnityEngine;

public class UI_Text : UIBase
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private UIAnimator _uiAnimator;

    public override void HideDirect()
    {

    }

    public override void Opened(params object[] param)
    {
        //필요한 값 초기화
        string inText = param[0].ToString();

        _text.text = inText;

        _uiAnimator.FadeOut();
        _uiAnimator.PopUp();
    }
}
