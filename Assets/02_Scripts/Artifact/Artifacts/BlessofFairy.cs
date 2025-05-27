
public class BlessofFairy : Artifact
{
    public BlessofFairy(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofFairy", 50, ModifierType.Precent);
        float originHP = player.MaxHealth;
        player.statBlock.AddModifier(StatType.MaxHealth, modifier);
        float afterHP = player.MaxHealth;
        player.Heal(afterHP - originHP);
    }

    public override void UnEquip(PlayerStats player)
    {
        player.statBlock.RemoveModifier(StatType.MaxHealth, modifier);
    }

}
