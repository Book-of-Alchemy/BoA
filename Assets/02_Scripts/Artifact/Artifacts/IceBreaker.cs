
public class IceBreaker : Artifact
{
    public IceBreaker(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveIceBreaker;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveIceBreaker;
    }

    public void ActiveIceBreaker(DamageInfo damageInfo)
    {
        foreach(StatusEffect ste in damageInfo.target.activeEffects)
        {
            if(ste.data.id == 220012)
            {
                modifier = new StatModifier("IceBreaker", 100, ModifierType.Precent);
                damageInfo.source.statBlock.AddModifier(StatType.FinalDmg, modifier);
                damageInfo.target.OnTakeDamage += RemoveIceBreakerModifier;
                break;
            }
        }
    }
    public void RemoveIceBreakerModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.FinalDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveIceBreakerModifier;
    }
}
