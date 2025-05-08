using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : StackableDebuff<Poison>
{
    public override void Tick(CharacterStats target)
    {
        float damage = value * stackCount;
        DamageInfo damageInfo = new DamageInfo(damage, DamageType.Dark, null, target);
        target.TakeDamage(damageInfo);
    }
}
