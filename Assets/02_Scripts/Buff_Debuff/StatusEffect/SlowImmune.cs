using System;
using System.Collections.Generic;

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
