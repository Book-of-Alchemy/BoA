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
    public List<Tag> tags;

    public DamageInfo(float value, DamageType damageType, CharacterStats source, CharacterStats target, bool isCritical, List<Tag> tags = null, int statusEffectID = -1)
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
            DamageType.Fire => target.fire,
            DamageType.Water => target.water,
            DamageType.Ice => target.ice,
            DamageType.Lightning => target.lightning,
            DamageType.Earth => target.earth,
            DamageType.Wind => target.wind,
            DamageType.Light => target.light,
            DamageType.Dark => target.dark,
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
        CharacterStats target = damageInfo.target;
        float baseDamage = damageInfo.value;
        DamageType damageType = DamageType.None;
        float result;
        float multiplier = damageType switch
        {
            DamageType.Fire => target.fire,
            DamageType.Water => target.water,
            DamageType.Ice => target.ice,
            DamageType.Lightning => target.lightning,
            DamageType.Earth => target.earth,
            DamageType.Wind => target.wind,
            DamageType.Light => target.light,
            DamageType.Dark => target.dark,
            _ => 0f
        };
        multiplier = Mathf.Max(0, (100 - multiplier) / 100f);

        result = multiplier * baseDamage;//속성 대미지 계산
        //버프 디버프에 의한 대미지 계산 추가
        result = result * (1 - target.defense / (50 + target.defense));//방어력 계산
        Debug.Log($"최종 대미지 : {result}");

        return result;
    }
}
