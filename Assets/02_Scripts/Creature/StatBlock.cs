using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StatType
{
    None,
}
public enum ModifierType
{
    flat,
    precent,
}
[System.Serializable]
public class StatModifier
{
    public string Source;
    public int Value;
    public ModifierType Type; // Flat, Percent
}
public class StatEntry
{
    public int BaseValue;
    private List<StatModifier> modifiers = new();

    public int Value
    {
        get
        {
            int flat = modifiers.Where(m => m.Type == ModifierType.flat).Sum(m => m.Value);
            float percent = modifiers.Where(m => m.Type == ModifierType.precent).Sum(m => m.Value) / 100f;
            return Mathf.FloorToInt((BaseValue + flat) * (1f + percent));
        }
    }

    public void AddModifier(string source, int value, ModifierType type)
    {
        RemoveModifier(source);
        modifiers.Add(new StatModifier { Source = source, Value = value, Type = type });
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(m => m.Source == source);
    }
}
public class StatBlock
{
    private Dictionary<StatType, StatEntry> stats = new();

    public StatBlock(Dictionary<StatType, int> baseStats)
    {
        foreach (var pair in baseStats)
            stats[pair.Key] = new StatEntry { BaseValue = pair.Value };
    }

    public int Get(StatType type) => stats[type].Value;

    public void AddModifier(StatType type, string source, int value, ModifierType modType)
    {
        stats[type].AddModifier(source, value, modType);
    }

    public void RemoveModifier(StatType type, string source)
    {
        stats[type].RemoveModifier(source);
    }

    public void SetBaseValue(StatType type, int baseValue)
    {
        stats[type].BaseValue = baseValue;
    }
}

