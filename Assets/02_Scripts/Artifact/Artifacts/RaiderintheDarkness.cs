
public class RaiderintheDarkness : Artifact
{
    public RaiderintheDarkness(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveRaiderintheDarkness;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveRaiderintheDarkness;
    }
    public void ActiveRaiderintheDarkness(DamageInfo damageInfo)
    {
        if (damageInfo.target.TryGetComponent<EnemyController>(out EnemyController targetController))
        {
            if (targetController._currentState == EnemyState.Idle)
            {
                modifier = new StatModifier("RaiderintheDarkness", 50, ModifierType.Precent);
                damageInfo.source.statBlock.AddModifier(StatType.DarkDmg, modifier);
                damageInfo.target.OnTakeDamage += RemoveRaiderintheDarknessModifier;
            }
        }
    }

    public void RemoveRaiderintheDarknessModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.DarkDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveRaiderintheDarknessModifier;
    }
}
