using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnImmune : Immune
{
    public BurnImmune(StatusEffectData data) : base(data, new HashSet<Type>() { typeof(Rooted) })
    {
        this.data = data;
    }
    public BurnImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) : base(data, new HashSet<Type>() { typeof(Rooted) })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}
