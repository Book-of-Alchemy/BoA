using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    None,
    Fire,
    Water,
    Ice,
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
            DamageType.Fire => target.fireDef,
            DamageType.Water => target.waterDef,
            DamageType.Ice => target.iceDef,
            DamageType.Lightning => target.lightningDef,
            DamageType.Earth => target.earthDef,
            DamageType.Wind => target.windDef,
            DamageType.Light => target.lightDef,
            DamageType.Dark => target.darkDef,
            _ => 0f
        };
        multiplier = Mathf.Max(0, (100 - multiplier) / 100f);

        result = multiplier * baseDamage;//속성 대미지 계산
        //버프 디버프에 의한 대미지 계산 추가
        result = result * (1 - target.defense / (50 + target.defense));//방어력 계산
        Debug.Log($"최종 대미지 : {result}");

        return result;
    }

    public static float CalculateDamage(DamageInfo damageInfo)
    {
        if(damageInfo.target == null)
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
                DamageType.Fire => source.fireDmg,
                DamageType.Water => source.waterDmg,
                DamageType.Ice => source.iceDmg,
                DamageType.Lightning => source.lightningDmg,
                DamageType.Earth => source.earthDmg,
                DamageType.Wind => source.windDmg,
                DamageType.Light => source.lightDmg,
                DamageType.Dark => source.darkDmg,
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
            DamageType.Fire => target.fireDef,
            DamageType.Water => target.waterDef,
            DamageType.Ice => target.iceDef,
            DamageType.Lightning => target.lightningDef,
            DamageType.Earth => target.earthDef,
            DamageType.Wind => target.windDef,
            DamageType.Light => target.lightDef,
            DamageType.Dark => target.darkDef,
            _ => 0f
        };
        elementalDef = Mathf.Max(0, (100 - elementalDef) / 100f);

        result = elementalDef * baseDamage;//속성 대미지 계산
        //버프 디버프에 의한 대미지 계산 추가
        result = result * (1 - target.defense / (50 + target.defense));//방어력 계산
        Debug.Log($"최종 대미지 : {result}");

        return result;
    }
}
