using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceofWind : Artifact
{
    public EssenceofWind(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EssenceofWind", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.WindResist, modifier);
        
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.WindResist, modifier);
    }
}
