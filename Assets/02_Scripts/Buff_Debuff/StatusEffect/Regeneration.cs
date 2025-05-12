using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regeneration : Buff
{
    public Regeneration(StatusEffectData data)
    {
        this.data = data;
    }
    public Regeneration(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if(!shouldRegister) return;

    }

    public override void Tick(CharacterStats target)
    {
        target.Heal(value);
    }
}
