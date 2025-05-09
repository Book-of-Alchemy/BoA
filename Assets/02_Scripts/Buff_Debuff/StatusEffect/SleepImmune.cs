using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepImmune : Immune
{
    public SleepImmune(StatusEffectData data) : base(data, new HashSet<Type>() { typeof(Asleep) })
    {
        this.data = data;
    }
    public SleepImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) : base(data, new HashSet<Type>() { typeof(Asleep) })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}
