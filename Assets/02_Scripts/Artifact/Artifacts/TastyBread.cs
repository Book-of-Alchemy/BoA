
public class TastyBread : Artifact
{
    public TastyBread(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("TastyBread",30,ModifierType.Precent);
        player.statBlock.AddModifier(StatType.MaxHealth, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        player.statBlock.RemoveModifier(StatType.MaxHealth, modifier);
    }
}
