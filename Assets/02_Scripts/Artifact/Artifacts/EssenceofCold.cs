
public class EssenceofCold : Artifact
{
    public EssenceofCold(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EssenceofCold", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.IceResist, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.IceResist, modifier);
    }
}
