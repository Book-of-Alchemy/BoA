
public class ShieldAmplifier : Artifact
{
    public ShieldAmplifier(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("ShieldAmplifier", 40, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.ShieldMultiplier, modifier);
    }
    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.ShieldMultiplier, modifier);
    }
}
