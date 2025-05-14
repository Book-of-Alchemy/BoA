using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedScroll : Artifact
{
    public EnhancedScroll(ArtifactData data) : base(data)
    {
    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EnhancedScroll", 15, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.ScrollDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.ScrollDmg, modifier);
    }
}
