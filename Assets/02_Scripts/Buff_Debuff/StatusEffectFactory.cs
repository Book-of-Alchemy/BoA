using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatusEffectFactory
{
    public static StatusEffect CreateEffect(int id)
    {
        if(SODataManager.Instance.StatusEffectDataBase == null)
            return null;

        Dictionary<int, StatusEffectData> kvp = SODataManager.Instance.StatusEffectDataBase.statusEffectsDataById;

        switch (id)
        {
            case 220001: return new AttackDown(kvp[id]);
            case 220002: return new DefenceDown(kvp[id]);
            case 220003: return new Rooted(kvp[id]);
            default: throw new Exception("Unknown status ID: " + id);
        }
    }
    public static StatusEffect CreateEffect(int id, int value = 10, int remainingTime = 30, int tickInterval = 10)
    {
        if (SODataManager.Instance.StatusEffectDataBase == null)
            return null;

        Dictionary<int, StatusEffectData> kvp = SODataManager.Instance.StatusEffectDataBase.statusEffectsDataById;

        switch (id)
        {
            case 220001: return new AttackDown(kvp[id],value,remainingTime,tickInterval);
            case 220002: return new DefenceDown(kvp[id], value, remainingTime, tickInterval);
            case 220003: return new Rooted(kvp[id], value, remainingTime, tickInterval);
            default: throw new Exception("Unknown status ID: " + id);
        }
    }
}