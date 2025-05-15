
public class EasyInstallationKit : Artifact
{
    public EasyInstallationKit(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.isEasyInstallationKit = true;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.isEasyInstallationKit = false;
    }
}
