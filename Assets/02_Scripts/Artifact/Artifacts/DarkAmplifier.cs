
public class DarkAmplifier : Artifact
{
    public DarkAmplifier(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("DarkAmplifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.DarkDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.DarkDmg, modifier);
    }
}
