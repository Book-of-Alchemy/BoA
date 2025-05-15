using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedThrowing : Artifact
{
    public EnhancedThrowing(ArtifactData data) : base(data)
    {
    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EnhancedThrowing", 15, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.ThrownDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.ThrownDmg, modifier);
    }
}
