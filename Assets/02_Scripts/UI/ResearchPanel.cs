using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResearchPanel : MonoBehaviour
{
    [SerializeField] private StatType _type;
    [SerializeField] private ResearchData _data;
    [SerializeField] private TextMeshProUGUI _researchVal; // 스탯에 실제 더해진 값
    [SerializeField] private TextMeshProUGUI _researchCnt; // 연구 레벨
    [SerializeField] private TextMeshProUGUI _researchGold;
    [SerializeField] private Button _upgradeBtn;


    private ResearchStat _statData;
    private Action<StatType> _OnUpgrade;

    public StatType GetStatType() => _type;
    public ResearchData GetResearchData() => _data;
    public void Initialize(ResearchStat stat, Action<StatType> onUpgrade)
    {
        _statData = stat;
        _OnUpgrade = onUpgrade;

        _upgradeBtn.onClick.RemoveAllListeners();
        _upgradeBtn.onClick.AddListener(() => _OnUpgrade?.Invoke(_type));
        UpdateUI();
    }

    public void UpdateUI()
    {
        _researchCnt.text = $"+{_statData.level}";
        //10은 임의 값 데이터 불러오면
        _researchVal.text = (_statData.level * _data.stat_value).ToString();
        _researchGold.text = _data.research_cost.ToString();

    }

}
