using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

enum EUIActionType
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
        _actionBtn.onClick.RemoveAllListeners();
        _backBtn.onClick.RemoveAllListeners();

        //int index = (int)param[2];
        InventoryItem item = (InventoryItem)param[2];
        
        //eUIActionType 타입과 생성위치 RectTransform으로 초기화
        if (param.Length > 0 && param[1] is RectTransform)
        {
            EUIActionType type = (EUIActionType)param[0]; //인자 Enum에 따른 다른 텍스트 출력과 역할
            Debug.Log(type);
            switch (type)
            {
                case EUIActionType.Use:
                    SetButtonText("Use","Drop");
                    AddButton(()=>Inventory.Instance.Use(item), ()=>Inventory.Instance.Drop(item));
                    break;
                case EUIActionType.Craft:
                    SetButtonText("Craft", "Cancel");
                    AddButton(() => Inventory.Instance.Craft(item), () => Inventory.Instance.Cancel(item));
                    break;
                case EUIActionType.Equip:
                    SetButtonText("Equip", "UnEquip");
                    AddButton(()=>Inventory.Instance.Equip(item), () => Inventory.Instance.UnEquip(item));
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

    private void SetPosition(RectTransform rectTransform)
    {
        //타겟 rectTrasnform 절반
        float Width = rectTransform.rect.width / 2;
        float Height = rectTransform.rect.height / 2;

        //Vector3로 rectTransform기준 우측하단에 배치
        _rectTransform.position = rectTransform.position + new Vector3(Width, -Height);
    }

    private void AddButton(UnityAction action, UnityAction back)
    {
        //버튼에 이벤트 등록
        _actionBtn.onClick.AddListener(action);
        _backBtn.onClick.AddListener(back);
    }

}
