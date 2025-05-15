using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedTrap : Artifact
{
    public EnhancedTrap(ArtifactData data) : base(data)
    {
    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EnhancedTrap", 15, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.TrapDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.TrapDmg, modifier);
    }
}
