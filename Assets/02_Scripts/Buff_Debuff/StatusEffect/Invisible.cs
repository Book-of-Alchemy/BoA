using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : Buff
{
    public Invisible(StatusEffectData data)
    {
        this.data = data;
    }
    public Invisible(StatusEffectData data, int value, int remainingTime, int tickInterval)
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
        target.IsHidden = true;
    }

    public override void OnExpire(CharacterStats target)
    {
        target.IsHidden = false;
    }
}
