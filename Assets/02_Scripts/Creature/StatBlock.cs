using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum StatType
{
    MaxHealth,
    MaxShield,
    MaxMana,
    Attack,
    Defence,
    CritChance,
    CritDamage,
    Evasion,
    Accuracy,
    VisionRange,
    AttackRange,
    FireResist,
    WaterResist,
    IceResist,
    LightningResist,
    EarthResist,
    WindResist,
    LightResist,
    DarkResist,
    FireDmg,
    WaterDmg,
    IceDmg,
    LightningDmg,
    EarthDmg,
    WindDmg,
    LightDmg,
    DarkDmg,
    ThrownDmg,
    TrapDmg,
    ScrollDmg,
    FinalDmg,
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
    public StatModifier() { }
    public StatModifier(string source, int value, ModifierType type)
    {
        this.source = source;
        this.value = value;
        this.type = type;
    }
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
                if (modifier.type == ModifierType.Flat)
                {
                    flat += modifier.value;
                }
                else if (modifier.type == ModifierType.Precent)
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
        modifiers.Add(new StatModifier(source, value, type));
        onStatChanged?.Invoke();
    }
    public void AddModifier(StatModifier statModifier)
    {
        modifiers.Add(statModifier);
        onStatChanged?.Invoke();
    }
    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(m => m.source == source);
        onStatChanged?.Invoke();
    }
    public void RemoveModifier(StatModifier statModifier)
    {
        modifiers.Remove(statModifier);
        onStatChanged?.Invoke();
    }
    public void SetBaseValue(int value)
    {
        baseValue = value;
        onStatChanged?.Invoke();
    }
}
[Serializable]
public class StatDictionary : SerializableDictionary<StatType, StatEntry> { }

[System.Serializable]
public class StatBlock
{
    [SerializeField]
    private StatDictionary stats = new();

    public StatBlock(Dictionary<StatType, int> baseStats)
    {
        foreach (var pair in baseStats)
            stats[pair.Key] = new StatEntry { baseValue = pair.Value };
    }

    public int Get(StatType type)
    {
        if (stats.TryGetValue(type, out var value))
            return value.Value;

        return 0;
    }
    public StatEntry GetEntry(StatType type) => stats[type];

    public void AddModifier(StatType type, string source, int value, ModifierType modType)
    {
        stats[type].AddModifier(source, value, modType);
    }

    public void AddModifier(StatType type, StatModifier statModifier)
    {
        stats[type].AddModifier(statModifier);
    }

    public void RemoveModifier(StatType type, string source)
    {
        stats[type].RemoveModifier(source);
    }
    public void RemoveModifier(StatType type, StatModifier statModifier)
    {
        stats[type].RemoveModifier(statModifier);
    }
    public void SetBaseValue(StatType type, int baseValue)
    {
        stats[type].SetBaseValue(baseValue);
    }
}

