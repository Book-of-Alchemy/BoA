
using UnityEngine;

public class UnstableGuardian : Artifact, ICooltime
{
    public int lefttime => Mathf.Max(0, availableTime - TurnManager.Instance.globalTime);
    public int coolTime { get; set; }
    public int availableTime { get ; set; }

    public UnstableGuardian(ArtifactData data) : base(data)
    {
        coolTime = 40;
        
    }
    // 아티팩트 획득시
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnPreTakeDamage += ActiveUnstableGuardian;
        player.OnTakeDamage += RemoveModifier;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnPreTakeDamage -= ActiveUnstableGuardian;
        player.OnTakeDamage -= RemoveModifier;
    }
    public void ActiveUnstableGuardian(DamageInfo damageInfo)
    {
        if(availableTime <= TurnManager.Instance.globalTime)
        {
            modifier = new StatModifier("UnstableGuardian", 50, ModifierType.Flat);
            damageInfo.target.statBlock.AddModifier(StatType.Defence, modifier);  
        }
    }

    public void RemoveModifier(DamageInfo damageInfo)
    {
        if(availableTime <= TurnManager.Instance.globalTime)
        {
            damageInfo.target.statBlock.RemoveModifier(StatType.Defence, modifier);
            availableTime = TurnManager.Instance.globalTime + coolTime;
        }
    }
}
