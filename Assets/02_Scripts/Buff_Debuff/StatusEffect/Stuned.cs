using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuned : Debuff
{
    UnitBase unit;
    public Stuned(StatusEffectData data)
    {
        this.data = data;
    }
    public Stuned(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
    public override void Tick(CharacterStats target)
    {
        if (unit == null || remainingTime <= tickInterval)
            return;

        unit.NextActionTime += 10;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        unit = target.GetComponent<UnitBase>();
        Tick(target);
    }
}
