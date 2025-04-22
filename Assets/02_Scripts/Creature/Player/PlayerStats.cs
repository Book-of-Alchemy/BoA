using System;
using UnityEngine;

public class PlayerStats : CharacterStats
{

    protected override void Awake()
    {
        base.Awake();
        // GameManager에 플레이어 등록
        GameManager.Instance.RegisterPlayer(this);
    }

    public void GainExperience(int exp)
    {
        experience += exp;
        Debug.Log(exp + "의 경험치 획득");
        // 레벨업 조건 확인 후 LevelUp 호출 가능
    }

    public void LevelUp()
    {
        level++;
        MaxHealth += 10f;
        CurrentHealth = MaxHealth;
        MaxMana += 5f;
        CurrentMana = MaxMana;
        attackMin += 1f;
        attackMax += 1f;
        defense += 1f;
        Debug.Log("레벨업 " + level);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
    }
}
