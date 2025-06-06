
public class EssemceofLight : Artifact
{
    public EssemceofLight(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EssenceofLight",20,ModifierType.Precent);
        player.statBlock.AddModifier(StatType.LightResist, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.LightResist, modifier);
    }
}
