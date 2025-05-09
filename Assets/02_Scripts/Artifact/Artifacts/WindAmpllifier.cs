using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAmpllifier : Artifact
{
    public WindAmpllifier(ArtifactData data) : base(data)
    {
    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("WindAmpllifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.WindAtk, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.WindAtk, modifier);
    }

}
