using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllImmune : Immune
{
    public AllImmune(StatusEffectData data) 
        : base(data, new HashSet<Type>() 
    { 
        typeof(Rooted),
        typeof(Stuned),
        typeof(Asleep),
        typeof(Confused),
        typeof(Blinded),
        typeof(Burn),
        typeof(Poison),
        typeof(Bleed),
        typeof(Shock),
    })
    {
        this.data = data;
    }
    public AllImmune(StatusEffectData data, int value, int remainingTime, int tickInterval) 
    : base(data, new HashSet<Type>() 
    { 
        typeof(Rooted),
        typeof(Stuned),
        typeof(Asleep),
        typeof(Confused),
        typeof(Blinded),
        typeof(Burn),
        typeof(Poison),
        typeof(Bleed),
        typeof(Shock),
    })
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
}


