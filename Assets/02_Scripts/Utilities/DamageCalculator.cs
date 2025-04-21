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
    Electric,
    Earth,
    Wind,
    Light,
    Dark,
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
            DamageType.Electric => target.electric,
            DamageType.Earth => target.earth,
            DamageType.Wind => target.wind,
            DamageType.Light => target.light,
            DamageType.Dark => target.dark,
            _ => 0f
        };
        multiplier = Mathf.Max(0f , 1f- multiplier);

        result = multiplier * baseDamage;//속성 대미지 계산
        //버프 디버프에 의한 대미지 계산 추가
        result = result * (target.defense / 50 + target.defense);//방어력 계산

        return result;
    }
}
