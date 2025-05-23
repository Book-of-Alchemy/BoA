
public class ManaOverload : Artifact
{
    public ManaOverload(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        GameManager.Instance.PlayerTransform.isManaOverload = true;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        GameManager.Instance.PlayerTransform.isManaOverload = false;
    }
}
