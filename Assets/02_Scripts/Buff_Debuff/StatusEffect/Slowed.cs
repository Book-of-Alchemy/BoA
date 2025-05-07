using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slowed : Debuff
{
    UnitBase unit;
    public Slowed(StatusEffectData data)
    {
        this.data = data;
    }
    public Slowed(StatusEffectData data, float value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        unit = target.GetComponent<UnitBase>();
        unit.nextActionTime += 5;
        //unit.actionCostStat.AddModifier("Slowed");
    }

    public override void OnExpire(CharacterStats target)
    {
        //unit.actionCost -= 5;
    }
}
