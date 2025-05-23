
public class LightningAmplifier : Artifact
{
    public LightningAmplifier(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("LightningAmplifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.LightningDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.LightningDmg, modifier);
    }
}
