using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : UIBase 
{
    [Header("Sliders")]
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Image _hpImage;
    [SerializeField] private Slider _mpSlider;
    [SerializeField] private Slider _expSlider;

    [SerializeField] private TextMeshProUGUI _levelVal;

    [SerializeField] private TextMeshProUGUI _floorTxt;
    [SerializeField] private TextMeshProUGUI _questTxt;

    [SerializeField] private List<QuickSlot> _quickSlots;

    private HUDPresenter _presenter;
    public override bool IsClosable => false;

    public override void HideDirect()
    {
        UIManager.Hide<UI_HUD>();
        QuestManager.Instance.OnQuestAccepted -= UpdateQuestTxt;
    }

    public override void Opened(params object[] param)
    {
        _presenter = new HUDPresenter(this);
        QuestManager.Instance.OnQuestAccepted += UpdateQuestTxt;
    }

    public void UpdateHp(float per)
    {
        _hpSlider.DOKill();
        _hpSlider.DOValue(per, 0.3f)
            .OnComplete(()=> _hpImage.DOFillAmount(per, 1.5f));
    }

    public void UpdateMp(float per)
    {
        _mpSlider.DOKill();
        _mpSlider.DOValue(per, 0.3f);
    }
    public void UpdateExp(float per)
    {
        _expSlider.DOKill();
        _expSlider.DOValue(per, 0.3f);
    }
    public void UpdateLevelTxt(int level)
    {
        _levelVal.text = $"Lv. {level}";
    }

    public void UpdateQuestTxt()
    {
        _questTxt.text = QuestManager.Instance.AcceptedQuest.Data.quest_name_kr;
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
        //UIManager.Show<UI_LvSelect>();
        //UIManager.Show<UI_SelectQuest>();
        UIManager.Show<UI_DungeonResult>();
    }

    private void OnDisable()
    {
        if(GameManager.Instance.PlayerTransform.TryGetComponent<PlayerStats>(out var player))
            player.OnLevelChanged -= UpdateLevelTxt;
    }
}
