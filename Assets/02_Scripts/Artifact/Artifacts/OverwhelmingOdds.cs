
public class OverwhelmingOdds : Artifact
{
    StatModifier modifier2;
    public OverwhelmingOdds(ArtifactData data) : base(data)
    {
    }
    // 아티팩트 획득시
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveOverwhelmingOdds;
        player.OnPreTakeDamage += ActiveOverwhelmingOddsDefence;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveOverwhelmingOdds;
    }

    // 공격데미지 증폭
    public void ActiveOverwhelmingOdds(DamageInfo damageInfo)
    {
        if ((damageInfo.source.CurrentHealth / damageInfo.source.MaxHealth) <= 0.3f)
        {
            modifier = new StatModifier("OverwhelmingOdds", 20, ModifierType.Precent);
            damageInfo.source.statBlock.AddModifier(StatType.FinalDmg, modifier);
            damageInfo.target.OnTakeDamage += RemoveOverwhelmingOddsModifier;
        }

    }
    public void RemoveOverwhelmingOddsModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.FinalDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveOverwhelmingOddsModifier;
    }

    // 피해량 감소
    public void ActiveOverwhelmingOddsDefence(DamageInfo damageInfo)
    {
        if (damageInfo.target== null) return;
        if ((damageInfo.target.CurrentHealth / damageInfo.target.MaxHealth) <= 0.3f)
        {
            modifier2 = new StatModifier("OverwhelmingOddsDefence", 10, ModifierType.Precent);
            damageInfo.target.statBlock.AddModifier(StatType.Defence, modifier2);
            damageInfo.target.OnTakeDamage += RemoveOverwhelmingOddsDefenceModifier;
        }
    }
    public void RemoveOverwhelmingOddsDefenceModifier(DamageInfo damageInfo)
    {
        damageInfo.target.statBlock.RemoveModifier(StatType.Defence, modifier2);
        damageInfo.target.OnTakeDamage -= RemoveOverwhelmingOddsDefenceModifier;
    }



}
