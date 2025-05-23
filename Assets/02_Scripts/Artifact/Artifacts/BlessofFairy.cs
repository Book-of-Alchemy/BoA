
public class BlessofFairy : Artifact
{
    public BlessofFairy(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofFairy", 50, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.MaxHealth, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        player.statBlock.RemoveModifier(StatType.MaxHealth, modifier);
    }

}
