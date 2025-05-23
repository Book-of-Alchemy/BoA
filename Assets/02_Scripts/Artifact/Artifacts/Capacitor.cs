
public class Capacitor : Artifact
{
    public Capacitor(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveCapacitor;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveCapacitor;
    }
    public void ActiveCapacitor(DamageInfo damageInfo)
    {
        foreach (StatusEffect ste in damageInfo.target.activeEffects)
        {
            if (ste.data.id == 220020)
            {
                modifier = new StatModifier("Capacitor", 50, ModifierType.Precent);
                damageInfo.source.statBlock.AddModifier(StatType.FinalDmg, modifier);
                damageInfo.target.OnTakeDamage += RemoveCapacitorModifier;
                break;
            }
        }
    }

    public void RemoveCapacitorModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.FinalDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveCapacitorModifier;
    }
}
