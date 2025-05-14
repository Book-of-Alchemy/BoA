using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthTraining : Artifact
{
    public StrengthTraining(ArtifactData data) : base(data)
    {
    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("StrengthTraining", 8*player.level, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.ThrownDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.ThrownDmg, modifier);
    }
}
