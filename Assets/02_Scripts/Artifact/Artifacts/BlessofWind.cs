
public class BlessofWind : Artifact
{
    public BlessofWind(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("BlessofWind", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.WindDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.WindDmg, modifier);
    }
}
