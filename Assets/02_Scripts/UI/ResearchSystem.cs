using System.Collections.Generic;

public class ResearchSystem : Singleton<ResearchSystem>
{
    private Dictionary<StatType, int> _researchLevels = new();
    private Dictionary<StatType, StatModifier> _activeModifiers = new();
    private ResearchDataBase _database;

    protected override void Awake()
    {
        base.Awake();
        _database = SODataManager.Instance.researchDataBase;
        InitializeFromSave();
    }

    public void InitializeFromSave()
    {
        _researchLevels.Clear();
        _activeModifiers.Clear();

        foreach (var data in DataManager.Instance.GetPlayerData().ResearchProgress)
        {
            _researchLevels[data.statType] = data.level;
        }
    }

    public void ApplyModifiersTo(CharacterStats stats)
    {
        foreach (var pair in _researchLevels)
        {
            var statType = pair.Key;
            var level = pair.Value;

            var data = _database.GetDataByStatType(statType);
            if (data == null) continue;

            int bonus = level * data.stat_value;

            var modifier = new StatModifier("Research", bonus, ModifierType.Flat);
            stats.statBlock.AddModifier(statType, modifier);
            _activeModifiers[statType] = modifier;
        }
    }

    public void UpdateStat(StatType statType, int newLevel, CharacterStats stats)
    {
        _researchLevels[statType] = newLevel;

        var data = _database.GetDataByStatType(statType);
        if (data == null) return;

        if (_activeModifiers.TryGetValue(statType, out var prevMod))
        {
            stats.statBlock.RemoveModifier(statType, prevMod);
        }

        int bonus = newLevel * data.stat_value;
        var modifier = new StatModifier("Research", bonus, ModifierType.Flat);
        stats.statBlock.AddModifier(statType, modifier);
        _activeModifiers[statType] = modifier;

        SaveToPlayerData();
    }

    private void SaveToPlayerData()
    {
        var saveList = new List<ResearchStat>();
        foreach (var pair in _researchLevels)
        {
            saveList.Add(new ResearchStat
            {
                statType = pair.Key,
                level = pair.Value
            });
        }

        DataManager.Instance.GetPlayerData().ResearchProgress = saveList;
        DataManager.Instance.SaveData();
    }
}