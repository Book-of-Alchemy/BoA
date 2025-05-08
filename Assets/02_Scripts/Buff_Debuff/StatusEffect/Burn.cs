using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : Debuff
{

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
        if (damageInfo.damageType == DamageType.Water || damageInfo.damageType == DamageType.Ice)
        {
            Cleanse();
        }
    }
    
}
