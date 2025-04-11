using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
    [Header("기본 스탯")]
    public int Level = 1;
    public int Experience = 0;

    [Header("체력 및 마나")]
    public float MaxHealth = 100f;
    public float CurrentHealth = 100f;
    public float MaxMana = 50f;
    public float CurrentMana = 50f;

    [Header("공격력")]
    public float AttackMin = 5f;
    public float AttackMax = 10f;

    [Header("방어 및 전투 스탯")]
    public float Defense = 5f;
    public float CritChance = 0.1f;   // 치명타 확률 10%
    public float CritDamage = 1.5f;   // 치명타 시 1.5배 데미지
    public float Evasion = 0.05f;     // 회피율 5%
    public float Accuracy = 1.0f;     // 명중률 기본값 1

    [Header("행동력")]
    public float ActionPoints = 1.0f;

    [Header("속성 (Elemental Attributes)")]
    public float Fire = 0f;     // 화염: 연소, 지속 피해(화상), 나무/식물 지형 상호작용
    public float Water = 0f;    // 물: 냉각, 체온 감소, 화염 제거
    public float Ice = 0f;      // 냉기: 체온 저하, 둔화, 얼림 등
    public float Electric = 0f; // 전기: 다단히트 효과, 감전 디버프
    public float Earth = 0f;    // 대지: 방해 지형 제거
    public float Wind = 0f;     // 바람: 넉백
    public float Light = 0f;    // 빛: 실명 등의 마법 효과
    public float Dark = 0f;     // 어둠: 혼란 등의 마법 효과

    /// <summary>
    /// 데미지 수치 만큼 체력 차감  
    /// 방어력 차감 후 최소 1 이상의 데미지 적용
    /// </summary>
    /// <param name="amount">입을 데미지</param>
    public virtual void TakeDamage(float amount)
    {
        float damage = Mathf.Max(amount - Defense, 1f);
        CurrentHealth -= damage;
        if (CurrentHealth < 0f)
        {
            CurrentHealth = 0f;
        }
        Debug.Log(gameObject.name + " took " + damage + " damage.");
    }

    /// <summary>
    /// 회복량 만큼 체력 증가  
    /// 최대치 초과 방지
    /// </summary>
    /// <param name="amount">회복량</param>
    public virtual void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        Debug.Log(gameObject.name + " healed " + amount + " health.");
    }
}
