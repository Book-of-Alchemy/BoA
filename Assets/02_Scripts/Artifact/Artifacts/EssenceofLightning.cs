using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceofLightning : Artifact
{
    public EssenceofLightning(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EssenceofLightning", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.ElectricResist, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.ElectricResist, modifier);
    }
}
