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
    [SerializeField]
    private float _actionPoints = 1.0f;

    public float ActionPoints
    {
        get { return _actionPoints; }
        set { _actionPoints = Mathf.Clamp(value, 0f, 1f); }
    }

    [Header("속성")]
    public float Fire = 0f;
    public float Water = 0f;
    public float Ice = 0f;
    public float Electric = 0f;
    public float Earth = 0f;
    public float Wind = 0f;
    public float Light = 0f;
    public float Dark = 0f;

    public virtual void TakeDamage(float amount)
    {
        float damage = Mathf.Max(amount - Defense, 1f);
        CurrentHealth -= damage;
        if (CurrentHealth < 0f)
        {
            CurrentHealth = 0f;
        }
        Debug.Log(gameObject.name + " 는 " + damage + " 의 데미지를 받음");
    }

    public virtual void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        Debug.Log(gameObject.name + " 는 " + amount + " 만큼 회복됨");
    }
}
