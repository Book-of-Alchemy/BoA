
public class BlessofCold : Artifact
{
    public BlessofCold(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofCold",20,ModifierType.Precent);
        player.statBlock.AddModifier(StatType.IceDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        player.statBlock.RemoveModifier(StatType.IceDmg,modifier);
    }
}
