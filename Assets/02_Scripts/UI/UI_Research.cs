using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResearchStat
{
    public StatType statType;
    public int level; // 강화 횟수
    public StatModifier modifier;
}

public class UI_Research : UIBase
{
    //연구 UI 패널 리스트
    [SerializeField] private List<ResearchPanel> _researchSlots;
    [SerializeField] private TextMeshProUGUI _goldText;

    // 강화 데이터 저장
    private Dictionary<StatType, ResearchStat> _researchStats = new();

    public override void Opened(params object[] param)
    {
        LoadResearchData();

        foreach (var slot in _researchSlots)
        {
            var statType = slot.GetStatType();

            if (!_researchStats.ContainsKey(statType)) //ResearchType이 없다면 ResearchStat생성
                _researchStats[statType] = new ResearchStat { statType = statType, level = 0 };

            slot.Initialize(_researchStats[statType],OnUpgrade);
        }
        Inventory.Instance.OnGoldChanged += UpdateGoldUI;
        UpdateGoldUI(Inventory.Instance.Gold);
    }

    public override void HideDirect()
    {
        Inventory.Instance.OnGoldChanged -= UpdateGoldUI;
        UIManager.Hide<UI_Research>();
    }

    private void OnUpgrade(StatType type)
    {
        //Panel의 타입 검사
        ResearchStat stat = _researchStats[type];
        var slot = _researchSlots.Find(s => s.GetStatType() == type);
        var data = slot.GetResearchData();

        if (stat.level >= data.max_level || Inventory.Instance.Gold < data.research_cost)
        {
            UIManager.ShowOnce<UI_Text>("골드 부족 또는 업그레이드 최대치 도달");
            return;
        }

        Inventory.Instance.DecreaseGold(data.research_cost);
        stat.level++;
        slot.UpdateUI();
        UpdateGoldUI(Inventory.Instance.Gold);

        var player = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
        ResearchSystem.Instance.UpdateStat(type, stat.level, player);
    }

    private void UpdateGoldUI(int amount)
    {
        _goldText.text = $"{amount:N0}";
    }

    private void LoadResearchData()
    {
        var savedList = DataManager.Instance.GetPlayerData().ResearchProgress;
        foreach (var saved in savedList)
        {
            _researchStats[saved.statType] = new ResearchStat
            {
                statType = saved.statType,
                level = saved.level
            };
        }
    }
}
