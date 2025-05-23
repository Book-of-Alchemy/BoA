using System;
using System.Collections.Generic;

public class StunImmune : Immune
{
    public StunImmune(StatusEffectData data) : base(data, new HashSet<Type>() { typeof(Stuned) })
    {
        this.data = data;
    }
    public StunImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) : base(data, new HashSet<Type>() { typeof(Stuned) })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}
