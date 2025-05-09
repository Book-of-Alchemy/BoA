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
            case 220004: statusEffect = new Stuned(StatusEffectDic[id]); break;
            case 220005: statusEffect = new Asleep(StatusEffectDic[id]); break;
            case 220006: statusEffect = new Slowed(StatusEffectDic[id]); break;
            case 220007: statusEffect = new Confused(StatusEffectDic[id]); break;
            case 220008: statusEffect = new Blinded(StatusEffectDic[id]); break;
            case 220009: statusEffect = new Burn(StatusEffectDic[id]); break;
            case 220010: statusEffect = new Poison(StatusEffectDic[id]); break;
            case 220011: statusEffect = new Bleed(StatusEffectDic[id]); break;
            case 220012: statusEffect = new Shock(StatusEffectDic[id]); break;
            case 220101: statusEffect = new AtkIncrease(StatusEffectDic[id]); break;
            case 220102: statusEffect = new DefIncrease(StatusEffectDic[id]); break;
            case 220103: statusEffect = new RootImmune(StatusEffectDic[id]); break;
            case 220104: statusEffect = new StunImmune(StatusEffectDic[id]); break;
            case 220105: statusEffect = new SleepImmune(StatusEffectDic[id]); break;
            case 220106: statusEffect = new SlowImmune(StatusEffectDic[id]); break;
            case 220107: statusEffect = new ConfuseImmune(StatusEffectDic[id]); break;
            case 220108: statusEffect = new BlindImmune(StatusEffectDic[id]); break;
            case 220109: statusEffect = new BurnImmune(StatusEffectDic[id]); break;
            case 220110: statusEffect = new PoisonImmune(StatusEffectDic[id]); break;
            case 220111: statusEffect = new BleedImmune(StatusEffectDic[id]); break;
            case 220112: statusEffect = new ShockImmune(StatusEffectDic[id]); break;
            case 220113: statusEffect = new Swift(StatusEffectDic[id]); break;
            case 220114: statusEffect = new AllImmune(StatusEffectDic[id]); break;
            case 220115: statusEffect = new Regeneration(StatusEffectDic[id]); break;
            case 220116: statusEffect = new Shield(StatusEffectDic[id]); break;
            case 220117: statusEffect = new Invincible(StatusEffectDic[id]); break;
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
            case 220004: statusEffect = new Stuned(StatusEffectDic[id]); break;
            case 220005: statusEffect = new Asleep(StatusEffectDic[id]); break;
            case 220006: statusEffect = new Slowed(StatusEffectDic[id]); break;
            case 220007: statusEffect = new Confused(StatusEffectDic[id]); break;
            case 220008: statusEffect = new Blinded(StatusEffectDic[id]); break;
            case 220009: statusEffect = new Burn(StatusEffectDic[id]); break;
            case 220010: statusEffect = new Poison(StatusEffectDic[id]); break;
            case 220011: statusEffect = new Bleed(StatusEffectDic[id]); break;
            case 220012: statusEffect = new Shock(StatusEffectDic[id]); break;
            case 220101: statusEffect = new AtkIncrease(StatusEffectDic[id]); break;
            case 220102: statusEffect = new DefIncrease(StatusEffectDic[id]); break;
            case 220103: statusEffect = new RootImmune(StatusEffectDic[id]); break;
            case 220104: statusEffect = new StunImmune(StatusEffectDic[id]); break;
            case 220105: statusEffect = new SleepImmune(StatusEffectDic[id]); break;
            case 220106: statusEffect = new SlowImmune(StatusEffectDic[id]); break;
            case 220107: statusEffect = new ConfuseImmune(StatusEffectDic[id]); break;
            case 220108: statusEffect = new BlindImmune(StatusEffectDic[id]); break;
            case 220109: statusEffect = new BurnImmune(StatusEffectDic[id]); break;
            case 220110: statusEffect = new PoisonImmune(StatusEffectDic[id]); break;
            case 220111: statusEffect = new BleedImmune(StatusEffectDic[id]); break;
            case 220112: statusEffect = new ShockImmune(StatusEffectDic[id]); break;
            case 220113: statusEffect = new Swift(StatusEffectDic[id]); break;
            case 220114: statusEffect = new AllImmune(StatusEffectDic[id]); break;
            case 220115: statusEffect = new Regeneration(StatusEffectDic[id]); break;
            case 220116: statusEffect = new Shield(StatusEffectDic[id]); break;
            case 220117: statusEffect = new Invincible(StatusEffectDic[id]); break;
            default: throw new Exception("Unknown status ID: " + id);
        }

        if (target != null)
            target.ApplyEffect(statusEffect);

        return statusEffect;
    }
}