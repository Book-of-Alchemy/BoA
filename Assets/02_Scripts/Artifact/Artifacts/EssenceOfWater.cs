using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceOfWater : Artifact
{
    public EssenceOfWater(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EssenceofWater", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.WaterDef, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.WaterDef, modifier);
    }
}
