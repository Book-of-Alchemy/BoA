using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAmplifier : Artifact
{
    public FireAmplifier(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("FireAmplifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.FireDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.FireDmg, modifier);
    }
}
