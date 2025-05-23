
public class Marksman : Artifact
{
    public Marksman(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.isMarksman = true;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.isMarksman = false;
    }
}
