using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkofRaider : Artifact
{
    public MarkofRaider(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveMarkofRaider;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveMarkofRaider;
    }
    public void ActiveMarkofRaider(DamageInfo damageInfo)
    {
        if ((damageInfo.source.CurrentHealth/damageInfo.source.MaxHealth) >= 0.9f)
        {
            modifier = new StatModifier("MarkofRaider", 25, ModifierType.Precent);
            damageInfo.source.statBlock.AddModifier(StatType.FinalDmg, modifier);
            damageInfo.target.OnTakeDamage += RemoveMarkofRaiderModifier;
        }

    }

    public void RemoveMarkofRaiderModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.FinalDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveMarkofRaiderModifier;
    }
}
