
public class WisdomofFairy : Artifact
{
    public WisdomofFairy(ArtifactData data) : base(data)
    {
    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("WisdomofFairy", 50, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.MaxMana, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        player.statBlock.RemoveModifier(StatType.MaxMana, modifier);
    }

}
