using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandoftheDrowned : Artifact
{
    public HandoftheDrowned(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveHandoftheDrowned;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveHandoftheDrowned;
    }
    public void ActiveHandoftheDrowned(DamageInfo damageInfo)
    {
        foreach (StatusEffect ste in damageInfo.target.activeEffects)
        {
            if (ste.data.id == 220014)
            {
                modifier = new StatModifier("HandoftheDrowned", 50, ModifierType.Precent);
                damageInfo.source.statBlock.AddModifier(StatType.FinalDmg, modifier);
                damageInfo.target.OnTakeDamage += RemoveHandoftheDrownedModifier;
                break;
            }
        }
    }

    public void RemoveHandoftheDrownedModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.FinalDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveHandoftheDrownedModifier;
    }
}
