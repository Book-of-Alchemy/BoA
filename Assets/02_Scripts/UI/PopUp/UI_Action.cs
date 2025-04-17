using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum eUIActionType
{
    Use,
    Craft,
    Equip
}

public class UI_Action : UIBase
{
    [SerializeField] private RectTransform _rectTransform;

    [Header("Buttons")]
    [SerializeField] private Button _actionBtn;
    [SerializeField] private TextMeshProUGUI _actionTxt;

    [SerializeField] private Button _backBtn;
    [SerializeField] private TextMeshProUGUI _backTxt;

    public override void HideDirect()
    {
        UIManager.Hide<UI_Action>();
    }

    public override void Opened(params object[] param) 
    {
        //eUIActionType 타입과 생성위치 RectTransform으로 초기화
        if (param.Length > 0 && param[0] is eUIActionType && param[1] is RectTransform)
        {
            eUIActionType type = (eUIActionType)param[0]; //인자 Enum에 따른 다른 텍스트 출력과 역할
            switch (type)
            {
                case eUIActionType.Use:
                    SetButtonText("Use","Drop");
                    break;
                case eUIActionType.Craft:
                    SetButtonText("Craft", "Cancel");
                    break;
                case eUIActionType.Equip:
                    SetButtonText("Equip", "UnEquip");
                    break;
            }
            SetPosition((RectTransform)param[1]);
        }
        else
        {
            Debug.LogError("Opened Param is Wrong");
            HideDirect();
        }
    }
    
    private void SetButtonText(string action, string back)
    {
        //버튼 표기 Text초기화
        _actionTxt.text = action;
        _backTxt.text = back;
    }

    public void SetPosition(RectTransform rectTransform)
    {
        //타겟 rectTrasnform 절반
        float Width = rectTransform.rect.width / 2;
        float Height = rectTransform.rect.height / 2;

        //Vector3로 rectTransform기준 우측하단에 배치
        _rectTransform.position = rectTransform.position + new Vector3(Width, -Height);
    }
}
