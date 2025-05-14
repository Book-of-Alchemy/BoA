// 상처 치유의 부적
public class AmuletofRestore : Artifact
{
    public AmuletofRestore(ArtifactData data) : base(data)
    {
    }
    // 아티팩트 획득시
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnHealthRatioChanged += ActiveAmuletofRestore;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnHealthRatioChanged -= ActiveAmuletofRestore;
    }

    // 회복력 증폭
    public void ActiveAmuletofRestore()
    {
        if(modifier != null)
            GameManager.Instance.PlayerTransform.statBlock.RemoveModifier(StatType.RegenerationMultiplier, modifier);

        int percent = (int)(1 - GameManager.Instance.PlayerTransform.CurrentHealth / GameManager.Instance.PlayerTransform.MaxHealth) * 100;
        modifier = new StatModifier("OverwhelmingOdds", percent, ModifierType.Precent);
        GameManager.Instance.PlayerTransform.statBlock.AddModifier(StatType.RegenerationMultiplier, modifier);
    }

}
