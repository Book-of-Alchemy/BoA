using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessofCold : Artifact
{
    public BlessofCold(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofCold",20,ModifierType.Precent);
        player.statBlock.AddModifier(StatType.IceAtk, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        player.statBlock.RemoveModifier(StatType.IceAtk,modifier);
    }
}
