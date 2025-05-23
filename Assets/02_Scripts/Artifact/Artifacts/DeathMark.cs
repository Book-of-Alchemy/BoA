
public class DeathMark : Artifact
{
    public DeathMark(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveDeathMark;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveDeathMark;
    }

    public void ActiveDeathMark(DamageInfo damageInfo)
    {
        if(damageInfo.target.activeEffects.Count>=3)
        {
            modifier = new StatModifier("DeathMark", 40, ModifierType.Precent);
            damageInfo.source.statBlock.AddModifier(StatType.FinalDmg, modifier);
            damageInfo.target.OnTakeDamage += RemoveDeathMarkModifier;
        }
    }

    public void RemoveDeathMarkModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.FinalDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveDeathMarkModifier;
    }
}
