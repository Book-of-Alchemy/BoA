using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectofFairy : Artifact
{
    public ProtectofFairy(ArtifactData data) : base(data)
    {
    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("ProtectofFairy", 15, ModifierType.Flat);
        player.statBlock.AddModifier(StatType.Defence, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.Defence, modifier);
    }
}
