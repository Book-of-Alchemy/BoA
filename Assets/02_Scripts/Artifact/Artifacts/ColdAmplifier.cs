
public class ColdAmplifier : Artifact
{
    public ColdAmplifier(ArtifactData data) : base(data)
    {
    }

    // Start is called before the first frame update
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("ColdAmplifier", 30, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.IceDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.IceDmg, modifier);
    }
}
