using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessofFire : Artifact
{
    public BlessofFire(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofFire", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.FireAtk, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.FireAtk, modifier);
    }
}
