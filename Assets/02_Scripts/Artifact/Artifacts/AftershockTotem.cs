using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AftershockTotem : Artifact
{
    public AftershockTotem(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveAftershockTotem;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
    }
    public void ActiveAftershockTotem(DamageInfo damageInfo)
    {
        if(damageInfo.damageType == DamageType.Earth)
        {
            StatusEffectFactory.CreateEffect(220008, 50, 30, 10, damageInfo.target);
        }
    }
}
