

public class MagicTraining : Artifact
{
    public MagicTraining(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        modifier = new StatModifier("MagicTraining", 8*player.level, ModifierType.Precent);
        player.statBlock.AddModifier(StatType.ScrollDmg, modifier);
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.statBlock.RemoveModifier(StatType.ScrollDmg, modifier);
    }
}
