using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Buff
{
    public override bool IsExpired => false;
    public Shield(StatusEffectData data)
    {
        this.data = data;
    }
    public Shield(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
    public override void OnApply(CharacterStats target)
    {
        this.target = target;
        foreach (var effect in target.activeEffects.ToArray())
        {
            if (effect.GetType() == this.GetType())
            {
                shouldRegister = false;
            }
        }
        target.GetShield(value);
        if(!shouldRegister)
            return;
        target.OnShieldChanged += CheckShield;

    }

    public override void OnExpire(CharacterStats target)
    {
        target.OnShieldChanged -= CheckShield;
    }

    protected virtual void CheckShield()
    {
        if (target.CurrentShield > 0)
            return;
        OnExpire(target);
    }
}
