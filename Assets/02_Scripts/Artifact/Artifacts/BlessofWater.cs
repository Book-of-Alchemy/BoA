
public class BlessofWater : Artifact
{
    public BlessofWater(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofWater", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.WaterDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.WaterDmg, modifier);
    }
}
