using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedAttack : Artifact
{
    public EnhancedAttack(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EnhancedAttack", 10, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.FinalDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.FinalDmg, modifier);
    }
}
