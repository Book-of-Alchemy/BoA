using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatusEffectFactory
{
    private static Dictionary<int, StatusEffectData> StatusEffectDic => SODataManager.Instance.StatusEffectDataBase.statusEffectsDataById;

    public static StatusEffect CreateEffect(int id, CharacterStats target = null)
    {
        if (StatusEffectDic == null)
            return null;

        StatusEffect statusEffect;
        switch (id)
        {
            case 220001: statusEffect = new AttackDown(StatusEffectDic[id]); break;
            case 220002: statusEffect = new DefenceDown(StatusEffectDic[id]); break;
            case 220003: statusEffect = new Rooted(StatusEffectDic[id]); break;
            default: throw new Exception("Unknown status ID: " + id);
        }

        if (target != null)
            target.ApplyEffect(statusEffect);

        return statusEffect;
    }
    public static StatusEffect CreateEffect(int id, int value = 10, int remainingTime = 30, int tickInterval = 10, CharacterStats target = null)
    {
        if (StatusEffectDic == null)
            return null;

        StatusEffect statusEffect;
        switch (id)
        {
            case 220001: statusEffect = new AttackDown(StatusEffectDic[id], value, remainingTime, tickInterval); break;
            case 220002: statusEffect = new DefenceDown(StatusEffectDic[id], value, remainingTime, tickInterval); break;
            case 220003: statusEffect = new Rooted(StatusEffectDic[id], value, remainingTime, tickInterval); break;
            default: throw new Exception("Unknown status ID: " + id);
        }

        if (target != null)
            target.ApplyEffect(statusEffect);

        return statusEffect;
    }
}