using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceofEarth : Artifact
{
    public EssenceofEarth(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new("EssenceofEarth", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.EarthResist, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.EarthResist, modifier);
    }
}
