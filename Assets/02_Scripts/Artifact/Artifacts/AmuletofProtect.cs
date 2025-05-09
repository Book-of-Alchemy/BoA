using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmuletofProtect : Artifact
{
    public AmuletofProtect(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("AmuletofProtect",10,ModifierType.Flat);
        player.statBlock.AddModifier(StatType.Defence, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.Defence, modifier);
    }
}
