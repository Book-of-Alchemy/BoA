
public class CraftTraining : Artifact
{
    public CraftTraining(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("CraftTraining", 8 * player.level, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.TrapDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.TrapDmg, modifier);
    }
}
