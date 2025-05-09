using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindImmune : Immune
{
    public BlindImmune(StatusEffectData data) : base(data, new HashSet<Type>() { typeof(Blinded) })
    {
        this.data = data;
    }
    public BlindImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) : base(data, new HashSet<Type>() { typeof(Blinded) })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}
