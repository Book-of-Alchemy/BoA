
public class LightAmplifier : Artifact
{
    public LightAmplifier(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("LightAmplifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.LightDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.LightDmg, modifier);
    }
}
