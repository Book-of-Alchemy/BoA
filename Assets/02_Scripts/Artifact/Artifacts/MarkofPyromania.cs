using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkofPyromania : Artifact
{
    public MarkofPyromania(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveMarkofPyromania;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveMarkofPyromania;
    }
    public void ActiveMarkofPyromania(DamageInfo damageInfo)
    {
        foreach (StatusEffect ste in damageInfo.target.activeEffects)
        {
            if (ste.data.id == 220017)
            {
                modifier = new StatModifier("MarkofPyromania", 50, ModifierType.Precent);
                damageInfo.source.statBlock.AddModifier(StatType.FinalDmg, modifier);
                damageInfo.target.OnTakeDamage += RemoveMarkofPyromaniaModifier;
                break;
            }
        }
    }

    public void RemoveMarkofPyromaniaModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.FinalDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveMarkofPyromaniaModifier;
    }
}
