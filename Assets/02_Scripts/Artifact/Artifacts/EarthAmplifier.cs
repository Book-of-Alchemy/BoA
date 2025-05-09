using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthAmplifier : Artifact
{
    public EarthAmplifier(ArtifactData data) : base(data)
    {
    }

    // Start is called before the first frame update
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EarthAmplifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.EarthDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.EarthDmg, modifier);
    }
}
