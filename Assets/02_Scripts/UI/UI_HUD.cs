using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : UIBase 
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Image _hpImage;

    [SerializeField] private Slider _mpSlider;
    [SerializeField] private Slider _expSlider;

    [SerializeField] private TextMeshProUGUI _floorTxt;
    [SerializeField] private TextMeshProUGUI _questTxt;

    [SerializeField] private List<QuickSlot> _quickSlots;

    private HUDPresenter _presenter;

    public override void HideDirect()
    {
        UIManager.Hide<UI_HUD>();
    }

    public override void Opened(params object[] param)
    {
        _presenter = new HUDPresenter(this);
    }

    public void UpdateHp(float per)
    {
        _hpSlider.DOValue(per, 0.3f)
            .OnComplete(()=> _hpImage.DOFillAmount(per, 1.5f));
    }

    public void UpdateMp(float per)
    {
        _mpSlider.DOValue(per, 0.3f);
    }
    public void UpdateExp(float per)
    {
        _expSlider.DOValue(per, 0.3f);
    }

    public void OnClickCraft() // Call At OnClick Event
    {
        UIManager.Show<UI_Inventory>(EInventoryType.Craft);
    }

    public void OnClickMenu() // Call At OnClick Event
    {
        UIManager.Show<UI_Menu>();
    }
    public void OnLevelUpBtn() // Call At OnClick Event
    {
        UIManager.Show<UI_LvSelect>();
    }
}
