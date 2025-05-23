
public class EssenceofLightning : Artifact
{
    public EssenceofLightning(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EssenceofLightning", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.LightningResist, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.LightningResist, modifier);
    }
}
