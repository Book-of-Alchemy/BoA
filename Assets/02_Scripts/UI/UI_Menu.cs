using UnityEngine;

public class UI_Menu : UIBase
{
    private UI_Inventory _Inventory;
    [SerializeField] private UIAnimator _uiAnimator; // Inspector 참조

    public override void HideDirect()
    {
        Debug.Log("Hide Menu");
        _uiAnimator.SlideTo(OnHide);
    }
    private void OnHide()
    {
        UIManager.Hide<UI_Menu>();
    }

    public override void Opened(params object[] param)
    {
        _Inventory = UIManager.Get<UI_Inventory>(); //인벤토리 캐싱
        _uiAnimator.SlideFrom();
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
    public void OnClickQuest()  // Call At OnClick Event
    {
        //UIManager.Show<>
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

    private void ShowInventory(EInventoryType inventoryType)// 인벤토리 타입에 따라 Show
    {
        UIManager.Show<UI_Inventory>(inventoryType);
        HideDirect();
    }

    private bool CheckInventoryType(EInventoryType inventoryType) 
    {
        if (_Inventory.CurType != inventoryType)
        {
            _Inventory.ShowRightTool(inventoryType);
            HideDirect();
            return true;
        }
        else
            return false;
    }
}
