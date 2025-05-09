using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfuseImmune : Immune
{
    public ConfuseImmune(StatusEffectData data) : base(data, new HashSet<Type>() { typeof(Confused) })
    {
        this.data = data;
    }
    public ConfuseImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) : base(data, new HashSet<Type>() { typeof(Confused) })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}
