using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    None,
    Fire,
    Water,
    Cold,
    Lightning,
    Earth,
    Wind,
    Light,
    Dark,
}

public struct DamageInfo
{
    public float value;
    public DamageType damageType;
    public CharacterStats source;  // 가해자
    public CharacterStats target;  // 피해자
    public bool isCritical;
    public int statusEffectID;
    public Tag[] tags;

    public DamageInfo
        (
        float value, 
        DamageType damageType, 
        CharacterStats source, 
        CharacterStats target, 
        bool isCritical = false,
        Tag[] tags = null, 
        int statusEffectID = -1
        )
    {
        this.value = value;
        this.damageType = damageType;
        this.source = source;
        this.target = target;
        this.isCritical = isCritical;
        this.statusEffectID = statusEffectID;
        this.tags = tags;
    }
}

public static class DamageCalculator
{
    public static float CalculateDamage(CharacterStats target, float baseDamage, DamageType damageType = DamageType.None)
    {
        float result;
        float multiplier = damageType switch
        {
            DamageType.Fire => target.FireResist,
            DamageType.Water => target.WaterResist,
            DamageType.Cold => target.IceResist,
            DamageType.Lightning => target.LightningResist,
            DamageType.Earth => target.EarthResist,
            DamageType.Wind => target.WindResist,
            DamageType.Light => target.LightResist,
            DamageType.Dark => target.DarkResist,
            _ => 0f
        };

        multiplier = Mathf.Max(0, (100 - multiplier) / 100f);

        result = multiplier * baseDamage;//속성 대미지 계산
        //버프 디버프에 의한 대미지 계산 추가
        result = result * (1 - target.Defence / (50 + target.Defence));//방어력 계산
        Debug.Log($"최종 대미지 : {result}");

        return result;
    }

    public static float CalculateDamage(DamageInfo damageInfo)
    {
        if (damageInfo.target == null)
            return 0f;
        CharacterStats source = damageInfo.source;
        CharacterStats target = damageInfo.target;
        float baseDamage = damageInfo.value;
        DamageType damageType = damageInfo.damageType;
        Tag[] tags = damageInfo.tags;
        bool isCrit = damageInfo.isCritical;
        float result;

        if (source != null)
        {
            float elementalDmg = damageType switch
            {
                DamageType.Fire => source.FireDmg,
                DamageType.Water => source.WaterDmg,
                DamageType.Cold => source.IceDmg,
                DamageType.Lightning => source.LightningDmg,
                DamageType.Earth => source.EarthDmg,
                DamageType.Wind => source.WindDmg,
                DamageType.Light => source.LightDmg,
                DamageType.Dark => source.DarkDmg,
                _ => 100f
            };
            elementalDmg /= 100f;
            float specialDmg = 1f;
            if (tags != null)
            {
                foreach (Tag tag in tags)
                {
                    switch (tag)
                    {
                        case Tag.Throw:
                            specialDmg *= source.ThrownDmg / 100f;
                            break;
                        case Tag.Scroll:
                            specialDmg *= source.ScrollDmg / 100f;
                            break;
                        case Tag.Trap:
                            specialDmg *= source.TrapDmg / 100f;
                            break;
                        default:
                            break;
                    }
                }
            }
            float finalDmg = source.FinalDmg / 100f;

            baseDamage = baseDamage * elementalDmg * specialDmg * finalDmg;
        }

        float elementalDef = damageType switch
        {
            DamageType.Fire => target.FireResist,
            DamageType.Water => target.WaterResist,
            DamageType.Cold => target.IceResist,
            DamageType.Lightning => target.LightningResist,
            DamageType.Earth => target.EarthResist,
            DamageType.Wind => target.WindResist,
            DamageType.Light => target.LightResist,
            DamageType.Dark => target.DarkResist,
            _ => 0f
        };
        elementalDef = Mathf.Max(0, (100 - elementalDef) / 100f);

        result = elementalDef * baseDamage;//속성 대미지 계산
        //버프 디버프에 의한 대미지 계산 추가
        result = result * (1 - target.Defence / (50f + (float)target.Defence));//방어력 계산
        //Debug.Log($"최종 대미지 : {result}");

        return result;
    }

    public static string GetIntroSoundID(Tag[] tags)
    {
        string result = "";
        foreach (Tag tag in tags)
        {
            result = tag switch
            {
                Tag.Throw => "throwing",
                Tag.Scroll => "scroll",
                _ => result,
            };
        }
        return result;
    }
    public static string GetImpactSoundID(Attribute attribute)
    {
        string result = "";

        result = attribute switch
        {
            Attribute.Fire => "fire",
            Attribute.Water => "scroll",
            Attribute.Cold => "cold",
            Attribute.Lightning => "lightning",
            Attribute.Earth => "earth",
            Attribute.Wind => "wind",
            Attribute.Light => "light",
            Attribute.Dark => "dark",
            Attribute.Oil => "water",
            _ => result,
        };

        return result;
    }
}
