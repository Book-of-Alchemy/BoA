using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonImmune : Immune
{
    public PoisonImmune(StatusEffectData data) : base(data, new HashSet<Type>() { typeof(Poison) })
    {
        this.data = data;
    }
    public PoisonImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) : base(data, new HashSet<Type>() { typeof(Poison) })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}