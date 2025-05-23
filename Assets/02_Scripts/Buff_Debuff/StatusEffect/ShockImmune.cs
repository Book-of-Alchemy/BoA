using System;
using System.Collections.Generic;

public class ShockImmune : Immune
{
    public ShockImmune(StatusEffectData data) : base(data, new HashSet<Type>() { typeof(Shock) })
    {
        this.data = data;
    }
    public ShockImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) : base(data, new HashSet<Type>() { typeof(Shock) })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}
