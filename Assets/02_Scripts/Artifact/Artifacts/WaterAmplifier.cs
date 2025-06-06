
public class WaterAmplifier : Artifact
{
    public WaterAmplifier(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("WaterAmplifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.WaterDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.WaterDmg, modifier);
    }
}
