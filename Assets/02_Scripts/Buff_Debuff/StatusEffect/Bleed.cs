using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bleed : StackableDebuff<Bleed>
{
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        this.target = target;
        target.OnTileChanged += AggravateBleed;
    }
    public override void OnExpire(CharacterStats target)
    {
        target.OnTileChanged -= AggravateBleed;
    }
    public override void Tick(CharacterStats target)
    {
        float damage = value * stackCount;
        DamageInfo damageInfo = new DamageInfo(damage, DamageType.None, null, target);
        target.TakeDamage(damageInfo);
    }
    public void AggravateBleed()
    {
        stackCount++;
        remainingTime += 10;
    }

}
