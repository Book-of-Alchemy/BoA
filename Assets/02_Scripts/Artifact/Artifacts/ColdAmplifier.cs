using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdAmplifier : Artifact
{
    public ColdAmplifier(ArtifactData data) : base(data)
    {
    }

    // Start is called before the first frame update
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("ColdAmplifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.IceAtk, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.IceAtk, modifier);
    }
}
