
public class BlessofEarth : Artifact
{
    public BlessofEarth(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofEarth", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.EarthDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.EarthDmg,modifier);
    }
}
