using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceIncrease : StatBuff
{
    public DefenceIncrease(StatusEffectData data)
    {
        this.data = data;
    }
    public DefenceIncrease(StatusEffectData data, int value, int remainingTime, int tickInterval)
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
        modifier = new StatModifier("DefenceIncrease", value, ModifierType.Flat);
        target.statBlock.AddModifier(StatType.Defence, modifier);
    }

    public override void OnExpire(CharacterStats target)
    {
        target.statBlock.RemoveModifier(StatType.Defence, modifier);
    }
}
