using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
    [Header("기본 스탯")]
    public int level = 1;
    public int experience = 0;

    [Header("체력 및 마나")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth = 100f;

    [Header("마나")]
    [SerializeField] protected float maxMana = 50f;
    [SerializeField] protected float currentMana = 50f;

    [Header("공격력")]
    public float attackMin = 5f;
    public float attackMax = 10f;

    [Header("방어 및 전투 스탯")]
    public float defense = 5f;
    public float critChance = 0.1f;
    public float critDamage = 1.5f;
    public float evasion = 0.05f;
    public float accuracy = 1.0f;

    [Header("행동력")]
    [SerializeField] private float actionPoints = 1.0f;
    public float ActionPoints
    {
        get => actionPoints;
        set => actionPoints = Mathf.Clamp(value, 0f, 1f);
    }

    [Header("시야")]
    public int visionRange = 6;
    public int attackRange = 1;

    [Header("Level & Tile")]
    public Level curLevel;
    public Tile curTile;
    public List<Tile> tilesOnVision => TileUtility.GetVisibleTiles(curLevel, curTile, visionRange);

    [Header("속성")]
    public float fire;
    public float water;
    public float ice;
    public float electric;
    public float earth;
    public float wind;
    public float light;
    public float dark;

    public BuffManager BuffManager { get; protected set; }
    private CharacterAnimator _anim;

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
    protected virtual void Awake()
    {
        _anim = GetComponent<CharacterAnimator>();
        BuffManager = GetComponent<BuffManager>() ?? gameObject.AddComponent<BuffManager>();
        // 초기값 전달
        OnHealthRatioChanged?.Invoke(CurrentHealth / MaxHealth);
    }

    public virtual void Attack(CharacterStats target)
    {
        float dmg = UnityEngine.Random.Range(attackMin, attackMax);
        if (UnityEngine.Random.value < critChance)
        {
            dmg *= critDamage;
            Debug.Log($"{gameObject.name}이(가) 치명타! 데미지: {dmg}");
        }
        Debug.Log($"{gameObject.name}이(가) {target.gameObject.name}을 공격합니다. 데미지: {dmg}");
        target.TakeDamage(dmg);
        _anim.PlayAttack();
    }

    public virtual void TakeDamage(float amount)
    {
        float dmg = Mathf.Max(amount - defense, 1f);
        CurrentHealth -= dmg;
        Debug.Log($"{gameObject.name}는 {dmg}의 피해를 받았습니다.");
        _anim.PlayKnockBack();
    }

    public virtual void Heal(float amount)
    {
        CurrentHealth += amount;
        Debug.Log($"{gameObject.name}는 {amount}만큼 회복되었습니다.");
    }

    public virtual void Die()
    {
        Debug.Log($"{gameObject.name}이(가) 사망했습니다.");
        _anim.PlayDeath();
    }

    public void OnMoveTile(Vector2Int start, Vector2Int target)
    {
        if (curLevel == null) return;
        if (curLevel.tiles.TryGetValue(start, out Tile startTile))
            startTile.CharacterStatsOnTile = null;

        if (curLevel.tiles.TryGetValue(target, out Tile targerTile))
        {
            targerTile.CharacterStatsOnTile = this as CharacterStats;
            curTile = targerTile;
        }
    }
}
