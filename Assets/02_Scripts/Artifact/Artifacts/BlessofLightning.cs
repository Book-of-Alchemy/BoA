using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessofLightning : Artifact
{
    public BlessofLightning(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofLightning", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.LightningDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.LightningDmg,modifier);
    }
}
