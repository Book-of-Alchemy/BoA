using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAmplifier : Artifact
{
    public LightAmplifier(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("LightAmplifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.LightAtk, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.LightAtk, modifier);
    }
}
