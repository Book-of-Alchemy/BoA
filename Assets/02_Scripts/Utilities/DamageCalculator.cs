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
            DamageType.Fire => target.Fire,
            DamageType.Water => target.Water,
            DamageType.Ice => target.Ice,
            DamageType.Electric => target.Electric,
            DamageType.Earth => target.Earth,
            DamageType.Wind => target.Wind,
            DamageType.Light => target.Light,
            DamageType.Dark => target.Dark,
            _ => 0f
        };
        multiplier = Mathf.Max(0f , 1f- multiplier);

        result = multiplier * baseDamage;//속성 대미지 계산
        //버프 디버프에 의한 대미지 계산 추가
        result = result * (1-target.Defense / (50 + target.Defense));//방어력 계산
        Debug.Log($"최종 대미지 : {result}");

        return result;
    }
}
