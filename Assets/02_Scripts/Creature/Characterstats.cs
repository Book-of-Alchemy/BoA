using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public abstract class CharacterStats : MonoBehaviour
{
    public StatBlock statBlock = new StatBlock(new Dictionary<StatType, int>
    {
        {StatType.MaxHealth, 100 },
        {StatType.MaxMana, 50 },
        {StatType.Attack, 10 },
        {StatType.Defence, 5 },
        {StatType.CritChance, 10 },
        {StatType.CritDamage, 150 },
        {StatType.Evasion, 5 },
        {StatType.Accuracy, 100 },
        {StatType.VisionRange, 6 },
        {StatType.AttackRange, 1 },
        {StatType.FireDef, 0 },
        {StatType.WaterDef, 0 },
        {StatType.IceDef, 0 },
        {StatType.ElectricDef, 0 },
        {StatType.EarthDef, 0 },
        {StatType.WindDef, 0 },
        {StatType.LightDef, 0 },
        {StatType.DarkDef, 0 },
        {StatType.FireAtk, 100 },
        {StatType.WaterAtk, 100 },
        {StatType.IceAtk, 100 },
        {StatType.ElectricAtk, 100 },
        {StatType.EarthAtk, 100 },
        {StatType.WindAtk, 100 },
        {StatType.LightAtk, 100 },
        {StatType.DarkAtk, 100 },
    });


    [Header("기본 스탯")]
    public int level = 1;
    public int experience = 0;

    [Header("체력 및 마나")]
    [SerializeField] protected float currentHealth = 100f;

    [Header("마나")]
    [SerializeField] protected float currentMana = 50f;

    //공격력
    public int AttackDamage => statBlock.Get(StatType.Attack);
    public int attackMin => Mathf.RoundToInt(AttackDamage * 0.9f);
    public int attackMax => Mathf.RoundToInt(AttackDamage * 1.1f);
    public int critChance => statBlock.Get(StatType.CritChance);
    public int critDamage => statBlock.Get(StatType.CritDamage);
    public int accuracy => statBlock.Get(StatType.Accuracy);

    //속성 공격

    //방어력
    public int defense => statBlock.Get(StatType.Defence);
    public int evasion => statBlock.Get(StatType.Evasion);

    //속성방어
    public int fire;
    public int water;
    public int ice;
    public int lightning;
    public int earth;
    public int wind;
    public int light;
    public int dark;


    //시야
    public int visionRange
    {
        get
        {
            int value = statBlock.Get(StatType.VisionRange);
            return value;
        }
    }
    public int attackRange => statBlock.Get(StatType.AttackRange);


    [Header("버프 디버프")]//위치 아래로 내릴것
    public List<StatusEffect> activeEffects = new();

    public bool hasImmunityToAll = false;

    [Header("Level & Tile")]
    public Level curLevel;
    [SerializeField]
    protected Tile curTile;
    public virtual Tile CurTile
    {
        get => curTile;
        set
        {
            curTile = value;
        }
    }
    public List<Tile> tilesOnVision => TileUtility.GetVisibleTiles(curLevel, CurTile, visionRange);


    public UnitBase unitBase { get; protected set; }
    private CharacterAnimator _anim;

    //체력 변경 이벤트
    public event Action OnHealthRatioChanged;
    public event Action OnManaChanged;
    public event Action OnTakeDamage;
    public float MaxHealth => statBlock.Get(StatType.MaxHealth);

    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0f, MaxHealth);
            OnHealthRatioChanged?.Invoke();
            if (currentHealth <= 0f) Die();
        }
    }
    public float MaxMana => statBlock.Get(StatType.MaxMana);

    public float CurrentMana
    {
        get => currentMana;
        set
        {
            currentMana = Mathf.Clamp(value, 0f, MaxMana);
            OnManaChanged?.Invoke();
        }
    }
    protected virtual void Awake()
    {
        _anim = GetComponent<CharacterAnimator>();
        unitBase = GetComponent<UnitBase>();
        statBlock.GetEntry(StatType.MaxHealth).onStatChanged += OnHealthRatioChanged;
        statBlock.GetEntry(StatType.MaxHealth).onStatChanged += OnManaChanged;
    }

    private void OnDestroy()
    {
        statBlock.GetEntry(StatType.MaxHealth).onStatChanged -= OnHealthRatioChanged;
        statBlock.GetEntry(StatType.MaxHealth).onStatChanged -= OnManaChanged;
    }


    /// <summary>
    /// 일반 공격의 경우
    /// </summary>
    public virtual void Attack(CharacterStats target, DamageType damageType = DamageType.None)
    {
        //기본 데미지 계산
        float baseDamage = UnityEngine.Random.Range(attackMin, attackMax);
        //치명타 게산
        bool isCrit = UnityEngine.Random.value < critChance / 100f;
        if (isCrit)
        {
            baseDamage *= critDamage / 100f;
            Debug.Log($"{gameObject.name}가 치명타!");
        }

        float finalDamage = DamageCalculator.CalculateDamage(target, baseDamage, damageType);
        target.TakeDamage(finalDamage);
        Debug.Log($"{gameObject.name}가 {target.gameObject.name}을 공격함니다." + $"속성:{damageType}, 최종 대미지:{finalDamage}");
    }
    /// <summary>
    /// 공격 배율이 있는경우
    /// </summary>
    public virtual void Attack(CharacterStats target, float multiplier, DamageType damageType = DamageType.None)
    {
        //기본 데미지 계산
        float baseDamage = UnityEngine.Random.Range(attackMin, attackMax) * multiplier;
        //치명타 게산
        bool isCrit = UnityEngine.Random.value < critChance / 100f;
        if (isCrit)
        {
            baseDamage *= critDamage / 100f;
            Debug.Log($"{gameObject.name}가 치명타!");
        }

        float finalDamage = DamageCalculator.CalculateDamage(target, baseDamage, damageType);
        target.TakeDamage(finalDamage, damageType);
        Debug.Log($"{gameObject.name}가 {target.gameObject.name}을 공격함니다." + $"속성:{damageType}, 최종 대미지:{finalDamage}");
    }
    public virtual void TakeDamage(float amount, DamageType damageType = DamageType.None)
    {
        CurrentHealth -= amount;
        UIManager.ShowOnce<DamageText>(amount, transform.position);
        Debug.Log($"{gameObject.name}는 {amount}의 피해를 받았습니다.");
        OnTakeDamage?.Invoke();
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

    public void MoveToTile(Tile targetTile)
    {
        if (curLevel == null || targetTile == null)
            return;

        // 이전 타일 점유 해제
        if (CurTile != null)
            CurTile.CharacterStatsOnTile = null;

        // 새 타일 점유 설정
        targetTile.CharacterStatsOnTile = this;

        //curTile 갱신
        CurTile = targetTile;
    }


    public void ApplyEffect(StatusEffect effect)
    {
        if (effect.IsStackable)
        {

        }

        effect.Initialize(TurnManager.Instance.globalTime);
        activeEffects.Add(effect);
        effect.OnApply(this);
        Debug.Log($"{name}에게 {effect.data.name_kr} 적용됨");
    }

    public void TickEffects(int globalTime)
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].TryTick(globalTime, this);
            if (activeEffects[i].IsExpired)
            {
                activeEffects[i].OnExpire(this);
                Debug.Log($"{name}의 {activeEffects[i].data.name_kr} 해제됨");
                activeEffects.RemoveAt(i);
            }
        }
    }

    public bool HasImmunity(Type effectType)
    {
        return activeEffects
            .OfType<IImunity>()
            .Any(im => im.BlockedTypes.Contains(effectType));
    }

    public void AffectOnTile(DamageInfo damageInfo)
    {

    }
}
