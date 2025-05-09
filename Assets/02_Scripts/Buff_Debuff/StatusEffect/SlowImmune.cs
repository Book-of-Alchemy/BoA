using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowImmune : Immune
{
    public SlowImmune(StatusEffectData data) : base(data, new HashSet<Type>() { typeof(Slowed) })
    {
        this.data = data;
    }
    public SlowImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) : base(data, new HashSet<Type>() { typeof(Slowed) })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}
