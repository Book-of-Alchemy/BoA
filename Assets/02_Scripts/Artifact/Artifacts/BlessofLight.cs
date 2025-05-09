using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessofLight : Artifact
{
    public BlessofLight(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofLight", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.LightDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.LightDmg,modifier);
    }
}
