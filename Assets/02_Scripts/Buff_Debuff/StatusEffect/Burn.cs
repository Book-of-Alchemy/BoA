using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : StackableDebuff<Burn>
{
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        target.OnTakeDamage += Cleanse;
    }
    public override void OnExpire(CharacterStats target)
    {
        target.OnTakeDamage -= Cleanse;
    }

    public override void Tick(CharacterStats target)
    {
        
        DamageInfo damageInfo = new DamageInfo();
    }

    protected void Cleanse(DamageInfo damageInfo)
    {
        if(damageInfo.damageType == DamageType.Water || damageInfo.damageType == DamageType.Ice)
        {
            OnExpire(damageInfo.target);
            damageInfo.target.activeEffects.Remove(this);
        }
    }
}
