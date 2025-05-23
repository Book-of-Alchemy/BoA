using System.Collections.Generic;
using UnityEngine;

public class StatusWindow : MonoBehaviour
{
    [SerializeField] private List<StatElement> _StatElements;

    private Dictionary<StatType, StatElement> _statUIDict;
    private PlayerStats _playerStats;

    private void OnEnable()
    {
        _playerStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
        _statUIDict = new();

        foreach (var element in _StatElements)
        {
            StatType type = element.StatType;
            element.SetName(GetLocalizedName(type));
            _statUIDict[type] = element;
        }

        if (_playerStats == null) return;
        UpdateAllStats();
    }

    public void UpdateAllStats()
    {
        foreach (var pair in _statUIDict)
        {
            float value = GetStatValueByProperty(pair.Key);
            pair.Value.SetValue(value);
        }
    }
    private float GetStatValueByProperty(StatType type)
    {
        return type switch
        {
            StatType.MaxHealth => _playerStats.MaxHealth,
            StatType.MaxMana => _playerStats.MaxMana,
            StatType.Attack => _playerStats.AttackDamage,
            StatType.Defence => _playerStats.Defence,
            StatType.FireDmg => _playerStats.FireDmg,
            StatType.WaterDmg => _playerStats.WaterDmg,
            StatType.IceDmg => _playerStats.IceDmg,
            StatType.LightningDmg => _playerStats.LightningDmg,
            StatType.EarthDmg => _playerStats.EarthDmg,
            StatType.WindDmg => _playerStats.WindDmg,
            StatType.LightDmg => _playerStats.LightDmg,
            StatType.DarkDmg => _playerStats.DarkDmg,
            StatType.ThrownDmg => _playerStats.ThrownDmg,
            StatType.TrapDmg => _playerStats.TrapDmg,
            StatType.ScrollDmg => _playerStats.ScrollDmg,
            _ => 0f
        };
    }
    private string GetLocalizedName(StatType type)
    {
        return type switch
        {
            StatType.MaxHealth => "체력",
            StatType.MaxMana => "마나",
            StatType.Attack => "공격력",
            StatType.Defence => "방어력",
            StatType.CritChance => "치명타 확률",
            StatType.CritDamage => "치명타 피해",
            StatType.ThrownDmg => "투척 공격력",
            StatType.ScrollDmg => "스크롤 공격력",
            StatType.TrapDmg => "함정 공격력",
            StatType.FireDmg => "화염 공격력",
            StatType.WaterDmg => "물 공격력",
            StatType.IceDmg => "얼음 공격력",
            StatType.LightningDmg => "전기 공격력",
            StatType.EarthDmg => "대지 공격력",
            StatType.WindDmg => "바람 공격력",
            StatType.LightDmg => "빛 공격력",
            StatType.DarkDmg => "어둠 공격력",
            _ => type.ToString()
        };
    }

}
