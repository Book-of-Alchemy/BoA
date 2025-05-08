using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkIncrease : StatBuff
{
   
    public AtkIncrease(StatusEffectData data)
    {
        this.data = data;
    }
    public AtkIncrease(StatusEffectData data, int value, int remainingTime, int tickInterval)
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
        modifier = new StatModifier("AttackIncrese", value, ModifierType.Flat);
        target.statBlock.AddModifier(StatType.Attack, modifier);
    }

    public override void OnExpire(CharacterStats target)
    {
        target.statBlock.RemoveModifier(StatType.Attack, modifier);
    }
}
