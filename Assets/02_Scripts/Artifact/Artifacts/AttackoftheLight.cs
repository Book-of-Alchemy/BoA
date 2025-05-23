
public class AttackoftheLight : Artifact
{
    public AttackoftheLight(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveAttackoftheLight;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveAttackoftheLight;
    }
    public void ActiveAttackoftheLight(DamageInfo damageInfo)
    {
        foreach (StatusEffect ste in damageInfo.target.activeEffects)
        {
            if (ste.data.id == 220016)
            {
                modifier = new StatModifier("AttackoftheLight", 50, ModifierType.Precent);
                damageInfo.source.statBlock.AddModifier(StatType.LightDmg, modifier);
                damageInfo.target.OnTakeDamage += RemoveAttackoftheLightModifier;
                break;
            }
        }
    }

    public void RemoveAttackoftheLightModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.LightDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveAttackoftheLightModifier;
    }
}
