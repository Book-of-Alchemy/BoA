
public class EssenceOfFire : Artifact
{
    public EssenceOfFire(ArtifactData data) : base(data)
    {

    }
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("EssenceOfFire", 20, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.FireResist, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.FireResist, modifier);
    }
}
