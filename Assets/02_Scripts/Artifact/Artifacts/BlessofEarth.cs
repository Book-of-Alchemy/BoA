using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessofEarth : Artifact
{
    public BlessofEarth(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofEarth", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.EarthAtk, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.EarthAtk,modifier);
    }
}
