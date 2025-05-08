using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDown : Debuff
{
    
    public AttackDown(StatusEffectData data)
    {
        this.data = data;
    }
    public AttackDown(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        modifier = new StatModifier("AttackDown", - value, ModifierType.Flat);
        target.statBlock.AddModifier(StatType.Attack, modifier);
    }

    public override void OnExpire(CharacterStats target)
    {
        target.statBlock.RemoveModifier(StatType.Attack, modifier);
    }
}
