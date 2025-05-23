using UnityEngine;

public class EmergencyStelthkit : Artifact, ICooltime
{
    public int lefttime => Mathf.Max(0, availableTime - TurnManager.Instance.globalTime);
    public int coolTime { get; set; }
    public int availableTime { get; set; }
    public EmergencyStelthkit(ArtifactData data) : base(data)
    {
        coolTime = 300;
    }
    // 아티팩트 획득시
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnTakeDamage += ActiveEmergencyStelthkit;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnTakeDamage -= ActiveEmergencyStelthkit;
    }
    public void ActiveEmergencyStelthkit(DamageInfo damageInfo)
    {
        if((damageInfo.target.CurrentHealth/damageInfo.target.MaxHealth)<=0.2f)
        {
            if(availableTime <= TurnManager.Instance.globalTime)
            {
                // 투명화
                StatusEffectFactory.CreateEffect(220124, GameManager.Instance.PlayerTransform);
                availableTime = TurnManager.Instance.globalTime + coolTime;
            }
        }
    }
}
