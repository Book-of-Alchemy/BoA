using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : Debuff
{
    public Burn(StatusEffectData data)
    {
        this.data = data;
    }
    public Burn(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        this.target = target;
        target.OnTakeDamage += TryCleanse;
    }
    public override void OnExpire(CharacterStats target)
    {
        target.OnTakeDamage -= TryCleanse;
    }

    public override void Tick(CharacterStats target)
    {
        float damage = value * 2f;
        DamageInfo damageInfo = new DamageInfo(damage, DamageType.Fire,null, target);
        target.TakeDamage(damageInfo);
    }

    protected void TryCleanse(DamageInfo damageInfo)
    {
        if (damageInfo.damageType == DamageType.Water || damageInfo.damageType == DamageType.Cold)
        {
            Cleanse();
        }
    }
    
}
