using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDown : Debuff
{
    public AttackDown(StatusEffectData data) 
    {
        this.data = data;
    }
    public AttackDown(StatusEffectData data,float value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        target.attackMax -= value;
        target.attackMin -= value;
    }

    public override void OnExpire(CharacterStats target)
    {
        target.attackMax += value;
        target.attackMin += value;
    }
}
