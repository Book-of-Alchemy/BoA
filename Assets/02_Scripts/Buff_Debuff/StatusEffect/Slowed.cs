using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slowed : Debuff
{
    UnitBase unit;
    public Slowed(StatusEffectData data)
    {
        this.data = data;
        value = 5;
    }
    public Slowed(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        unit = target.unitBase;
        unit.nextActionTime += 5;
        modifier = new StatModifier("Slowed", - value, ModifierType.Flat);
        unit.actionCostStat.AddModifier(modifier);
    }

    public override void OnExpire(CharacterStats target)
    {
        unit.actionCostStat.RemoveModifier(modifier);
    }
}
