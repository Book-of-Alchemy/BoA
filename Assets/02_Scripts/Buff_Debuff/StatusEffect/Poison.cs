using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : StackableDebuff<Poison>
{
    public Poison(StatusEffectData data)
    {
        this.data = data;
    }
    public Poison(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
    public override void Tick(CharacterStats target)
    {
        float damage = value * stackCount;
        DamageInfo damageInfo = new DamageInfo(damage, DamageType.Dark, null, target);
        target.TakeDamage(damageInfo);
    }
}
