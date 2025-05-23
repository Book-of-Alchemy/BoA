using System;
using System.Collections.Generic;

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
