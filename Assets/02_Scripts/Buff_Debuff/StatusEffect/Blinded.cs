using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinded : Debuff
{
    public Blinded(StatusEffectData data)
    {
        this.data = data;
    }
    public Blinded(StatusEffectData data, int value, int remainingTime, int tickInterval)
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
        modifier = new StatModifier("Blinded", -value, ModifierType.Flat);
        target.statBlock.AddModifier(StatType.VisionRange, modifier);
    }

    public override void OnExpire(CharacterStats target)
    {
        target.statBlock.RemoveModifier(StatType.VisionRange, modifier);
    }
}
