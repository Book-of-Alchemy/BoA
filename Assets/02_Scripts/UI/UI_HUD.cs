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
    [SerializeField] private TextMeshProUGUI _goldTxt;

    [SerializeField] private List<QuickSlot> _quickSlots;
    
    [Header("상태 효과 표시")]
    [SerializeField] private GameObject statusEffectDisplayPrefab;
    [SerializeField] private Transform statusEffectContainer;
    private UI_StatusEffectDisplay _statusEffectDisplay;

    private HUDPresenter _presenter;
    public override bool IsClosable => false;

    public override void HideDirect()
    {
        UIManager.Hide<UI_HUD>();
        QuestManager.Instance.OnQuestAccepted -= UpdateQuestTxt;
        Inventory.Instance.OnGoldChanged -= UpdateGoldUI;
        TileManger.OnGetDown -= UpdateFloorTxt;
    }

    public override void Opened(params object[] param)
    {
        _presenter = new HUDPresenter(this);
        QuestManager.Instance.OnQuestAccepted += UpdateQuestTxt;
        UpdateQuestTxt();

        TileManger.OnGetDown += UpdateFloorTxt;

        //Gold 변화 구독
        Inventory.Instance.OnGoldChanged += UpdateGoldUI;
        UpdateGoldUI(Inventory.Instance.Gold);

        // 상태 효과 디스플레이 초기화
        InitStatusEffectDisplay();
    }
    
    private void InitStatusEffectDisplay()
    {
        if (statusEffectContainer != null && statusEffectDisplayPrefab != null)
        {
            // 기존에 있다면 제거
            if (_statusEffectDisplay != null)
            {
                Destroy(_statusEffectDisplay.gameObject);
            }
            
            // 새로 생성
            GameObject statusDisplayObj = Instantiate(statusEffectDisplayPrefab, statusEffectContainer);
            _statusEffectDisplay = statusDisplayObj.GetComponent<UI_StatusEffectDisplay>();
            
            // 플레이어 연결 (이미 내부에서 자동으로 찾지만 명시적으로도 설정 가능)
            if (GameManager.Instance != null && GameManager.Instance.PlayerTransform != null)
            {
                PlayerStats playerStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
                if (_statusEffectDisplay && playerStats != null)
                {
                    _statusEffectDisplay.TargetStats = playerStats;
                }
            }
        }
    }
    public void UpdateFloorTxt(int floor)
    {
        _floorTxt.text = $"{floor+1}층";
    }
    private void UpdateGoldUI(int amount)
    {
        _goldTxt.text = $"{amount:N0}G";
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
        if(QuestManager.Instance.AcceptedQuest != null)
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
        UIManager.Show<UI_LvSelect>();
        
    }

    public void OnSelectQuest()
    {
        UIManager.Show<UI_SelectQuest>();
    }

    public void OnDungeonResult()
    {
        UIManager.Show<UI_DungeonResult>();
    }
    public void OnReserach()
    {
        UIManager.Show<UI_Research>();
    }

    private void OnDisable()
    {
        //던전-마을로 씬전환할때 UI_HUD가 비활성화 되면서 이 메서드가 호출되는데 플레이어가 파괴된 상태에서 접근을 시도하여 오류가 발생함.
        //널체크 추가했습니다.
        if(GameManager.Instance != null 
            && GameManager.Instance.PlayerTransform != null 
            && GameManager.Instance.PlayerTransform.TryGetComponent<PlayerStats>(out var player))
        {
            player.OnLevelChanged -= UpdateLevelTxt;
        }
    }
}
