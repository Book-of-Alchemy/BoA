using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceofDark : Artifact
{
    public EssenceofDark(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EssenceofDark", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.DarkResist, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.DarkResist, modifier);
    }
}
