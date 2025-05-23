
public class Purity : Artifact
{
    public Purity(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActivePurity;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActivePurity;
    }
    public void ActivePurity(DamageInfo damageInfo)
    {
        if(damageInfo.statusEffectID == -1)
        {
            modifier = new StatModifier("Purity", 30, ModifierType.Precent);
            damageInfo.source.statBlock.AddModifier(StatType.FinalDmg, modifier);
            damageInfo.target.OnTakeDamage += RemovePurityModifier;
        }
    }
    public void RemovePurityModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.FinalDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemovePurityModifier;
    }
}
