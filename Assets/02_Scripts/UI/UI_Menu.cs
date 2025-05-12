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
        if (_Inventory != null)
            _Inventory.HideDirect();
        _uiAnimator.SlideFrom();
    }
    private void ShowInventory(EInventoryType inventoryType)// 인벤토리 타입에 따라 Show
    {
        UIManager.Show<UI_Inventory>(inventoryType);
        HideDirect();
    }

    // Inspector 등록 버튼이벤트
    public void OnClickStatus() => ShowInventory(EInventoryType.Status);//열려있지 않은경우
    public void OnClickEquipment() => ShowInventory(EInventoryType.Equipment);
    public void OnClickCraft() => ShowInventory(EInventoryType.Craft);
    public void OnClickInventory() => ShowInventory(EInventoryType.Inventory);
    public void OnClickQuest() => ShowInventory(EInventoryType.Quest);
    public void OnClickMap() => ShowInventory(EInventoryType.Map);
    public void OnClickArtifact() => ShowInventory(EInventoryType.Artifact);

    public void OnClickSetting()
    {
        UIManager.Show<UI_Setting>();
        HideDirect();
    }

    public void OnClickMainMenu() 
    {
        UIManager.Show<UI_Main>();
        HideDirect();
    }
}
