using System;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    //체력 변경 이벤트
    public event Action<float> OnHealthRatioChanged;
    // BuffManager를 통해 상태 효과를 관리할 수 있도록 프로퍼티 추가
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = Mathf.Max(1f, value);
            CurrentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthRatioChanged?.Invoke(currentHealth / maxHealth);
        }
    }
    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);
            OnHealthRatioChanged?.Invoke(currentHealth / maxHealth);
            if (currentHealth <= 0f) Die();
        }
    }
    public float MaxMana
    {
        get => maxMana;
        set => maxMana = Mathf.Max(0f, value);
    }
    public float CurrentMana
    {
        get => currentMana;
        set => currentMana = Mathf.Clamp(value, 0f, maxMana);
    }
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
