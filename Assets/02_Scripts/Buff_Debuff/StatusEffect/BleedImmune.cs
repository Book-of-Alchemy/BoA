using System;
using System.Collections.Generic;

public class BleedImmune : Immune
{
    public BleedImmune(StatusEffectData data) : base(data, new HashSet<Type>() { typeof(Bleed) })
    {
        this.data = data;
    }
    public BleedImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) : base(data, new HashSet<Type>() { typeof(Bleed) })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}