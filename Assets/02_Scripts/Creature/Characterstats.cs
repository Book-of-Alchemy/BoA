using System.Collections.Generic;
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

    [Header("시야")]
    public int visionRange = 6;
    public int attackRange = 1;

    [Header("Level&Tile")]
    public Level curLevel;
    public Tile curTile;
    public List<Tile> TilesOnVision => TileUtility.GetVisibleTiles(curLevel, curTile, visionRange);

    [Header("속성")]
    public float Fire = 0f;
    public float Water = 0f;
    public float Ice = 0f;
    public float Electric = 0f;
    public float Earth = 0f;
    public float Wind = 0f;
    public float Light = 0f;
    public float Dark = 0f;

    public BuffManager BuffManager { get; protected set; }
    protected virtual void Awake()
    {
        BuffManager = GetComponent<BuffManager>();
        if (BuffManager == null)
            BuffManager = gameObject.AddComponent<BuffManager>();
    }

    public virtual void Attack(CharacterStats target)
    {
        // 최소/최대 공격력 사이에서 랜덤 데미지 계산
        float damage = Random.Range(AttackMin, AttackMax);

        // 치명타 발생 체크
        if (Random.value < CritChance)
        {
            damage *= CritDamage;
            Debug.Log(gameObject.name + "이(가) 치명적인 공격을 했습니다!");
        }

        Debug.Log(gameObject.name + "이(가) " + target.gameObject.name + "을 공격합니다. (데미지: " + damage + ")");
        target.TakeDamage(damage);
    }

    public virtual void TakeDamage(float amount)
    {
        float damage = Mathf.Max(amount - Defense, 1f);
        CurrentHealth -= damage;
        if (CurrentHealth < 0f)
        {
            CurrentHealth = 0f;
            Die();
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

    public virtual void Die()
    {
        if (CurrentHealth == 0f)
        {
            Debug.Log(gameObject.name + "(이)가 사망함");
        }
    }

    public void OnMoveTile(Vector2Int start, Vector2Int target)
    {
        if (curLevel == null) return;

        if (curLevel.tiles.TryGetValue(start, out Tile startTile))
            startTile.characterStats = null;

        if (curLevel.tiles.TryGetValue(target, out Tile targerTile))
        {
            targerTile.characterStats = this as CharacterStats;
            curTile = targerTile;
        }
    }
}
