using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceDown : Debuff
{
    public DefenceDown(StatusEffectData data)
    {
        this.data = data;
    }
    public DefenceDown(StatusEffectData data, float value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        target.defense -= value;
    }

    public override void OnExpire(CharacterStats target)
    {
        target.defense += value;
    }
}
