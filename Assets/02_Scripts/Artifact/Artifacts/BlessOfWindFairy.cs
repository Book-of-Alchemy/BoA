
public class BlessOfWindFairy : Artifact
{
    public BlessOfWindFairy(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveBlessOfWindFairy;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveBlessOfWindFairy;
    }
    public void ActiveBlessOfWindFairy(DamageInfo damageInfo)
    {
        foreach (StatusEffect ste in damageInfo.source.activeEffects)
        {
            if (ste.data.id == 220121)
            {
                modifier = new StatModifier("BlessOfWindFairy", 50, ModifierType.Precent);
                damageInfo.source.statBlock.AddModifier(StatType.FinalDmg, modifier);
                damageInfo.target.OnTakeDamage += RemoveBlessOfWindFairyModifier;
                break;
            }
        }
    }

    public void RemoveBlessOfWindFairyModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.FinalDmg,modifier);
        damageInfo.target.OnTakeDamage -= RemoveBlessOfWindFairyModifier;
    }
}
