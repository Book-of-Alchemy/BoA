
public class BlessofDark : Artifact
{
    public BlessofDark(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofDark", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.DarkDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.DarkDmg, modifier);
    }
}
