using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    MaxHealth,
    MaxMana,
    Attack,
    Defence,
    CritChance,
    CritDamage,
    Evasion,
    Accuracy,
    VisionRange,
    AttackRange,
    FireDef,
    WaterDef,
    IceDef,
    ElectricDef,
    EarthDef,
    WindDef,
    LightDef,
    DarkDef,
    FireAtk,
    WaterAtk,
    IceAtk,
    ElectricAtk,
    EarthAtk,
    WindAtk,
    LightAtk,
    DarkAtk,
}
public enum ModifierType
{
    Flat,
    Precent,
}
[System.Serializable]
public class StatModifier
{
    public string source;
    public int value;
    public ModifierType type; // Flat, Percent
}

[System.Serializable]
public class StatEntry
{
    public event Action onStatChanged;
    public int baseValue;
    private List<StatModifier> modifiers = new();

    public int Value
    {
        get
        {
            int flat = 0;
            float percent = 0f;
            foreach (StatModifier modifier in modifiers)
            {
                if(modifier.type == ModifierType.Flat)
                {
                    flat += modifier.value;
                }
                else if(modifier.type == ModifierType.Precent)
                {
                    percent += modifier.value;
                }
            }
            percent /= 100f;
            return Mathf.FloorToInt((baseValue + flat) * (1f + percent));
        }
    }

    public void AddModifier(string source, int value, ModifierType type)
    {
        RemoveModifier(source);
        modifiers.Add(new StatModifier { source = source, value = value, type = type });
        onStatChanged?.Invoke();
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(m => m.source == source);
        onStatChanged?.Invoke();
    }

    public void SetBaseValue(int value)
    {
        baseValue = value;
        onStatChanged?.Invoke();
    }
}
[System.Serializable]
public class StatBlock
{
    private Dictionary<StatType, StatEntry> stats = new();

    public StatBlock(Dictionary<StatType, int> baseStats)
    {
        foreach (var pair in baseStats)
            stats[pair.Key] = new StatEntry { baseValue = pair.Value };
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
        stats[type].SetBaseValue(baseValue);
    }
}

