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
        {StatType.FireResist, 0 },
        {StatType.WaterResist, 0 },
        {StatType.IceResist, 0 },
        {StatType.LightningResist, 0 },
        {StatType.EarthResist, 0 },
        {StatType.WindResist, 0 },
        {StatType.LightResist, 0 },
        {StatType.DarkResist, 0 },
        {StatType.FireDmg, 100 },
        {StatType.WaterDmg, 100 },
        {StatType.IceDmg, 100 },
        {StatType.LightningDmg, 100 },
        {StatType.EarthDmg, 100 },
        {StatType.WindDmg, 100 },
        {StatType.LightDmg, 100 },
        {StatType.DarkDmg, 100 },
        {StatType.ThrownDmg, 100 },
        {StatType.TrapDmg, 100 },
        {StatType.ScrollDmg, 100 },
        {StatType.FinalDmg, 100 },
        {StatType.ShieldMultiplier, 100 },
        {StatType.RegenerationMultiplier, 100 },
    });


    [Header("기본 스탯")]
    public int level = 1;
    public int experience = 0;

    //체력 쉴드 마나
    public float MaxHealth => statBlock.Get(StatType.MaxHealth);
    [SerializeField] protected float currentHealth = 100f;
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
    [SerializeField] protected float currentShield = 0f;
    public float CurrentShield 
    { 
        get => currentShield;
        set
        { 
            currentShield = MathF.Max(0, value);
            OnShieldChanged?.Invoke();
        } 
    }
    public float MaxMana => statBlock.Get(StatType.MaxMana);
    [SerializeField] protected float currentMana = 50f;
    public float CurrentMana
    {
        get => currentMana;
        set
        {
            currentMana = Mathf.Clamp(value, 0f, MaxMana);
            OnManaChanged?.Invoke();
        }
    }
    //쉴드 체력 재생
    public int RegenerationMultiplier => statBlock.Get(StatType.RegenerationMultiplier);
    public int ShieldMultiplier => statBlock.Get(StatType.ShieldMultiplier);


    //공격력
    public int AttackDamage => Mathf.Max(0, statBlock.Get(StatType.Attack));
    public int AttackMin => Mathf.RoundToInt(AttackDamage * 0.9f);
    public int AttackMax => Mathf.RoundToInt(AttackDamage * 1.1f);
    public int CritChance => statBlock.Get(StatType.CritChance);
    public int CritDamage => statBlock.Get(StatType.CritDamage);
    public int Accuracy => statBlock.Get(StatType.Accuracy);

    //속성 공격

    public int FireDmg => statBlock.Get(StatType.FireDmg);
    public int WaterDmg => statBlock.Get(StatType.WaterDmg);
    public int IceDmg => statBlock.Get(StatType.IceDmg);
    public int LightningDmg => statBlock.Get(StatType.LightningDmg);
    public int EarthDmg => statBlock.Get(StatType.EarthDmg);
    public int WindDmg => statBlock.Get(StatType.WindDmg);
    public int LightDmg => statBlock.Get(StatType.LightDmg);
    public int DarkDmg => statBlock.Get(StatType.DarkDmg);

    //특성공격력

    public int ThrownDmg => statBlock.Get(StatType.ThrownDmg);
    public int TrapDmg => statBlock.Get(StatType.TrapDmg);
    public int ScrollDmg => statBlock.Get(StatType.ScrollDmg);
    public int FinalDmg => statBlock.Get(StatType.FinalDmg);

    //방어력
    public int Defence => statBlock.Get(StatType.Defence);
    public int Evasion => statBlock.Get(StatType.Evasion);

    //속성방어
    public int FireResist => statBlock.Get(StatType.FireResist);
    public int WaterResist => statBlock.Get(StatType.WaterResist);
    public int IceResist => statBlock.Get(StatType.IceResist);
    public int LightningResist => statBlock.Get(StatType.LightningResist);
    public int EarthResist => statBlock.Get(StatType.EarthResist);
    public int WindResist => statBlock.Get(StatType.WindResist);
    public int LightResist => statBlock.Get(StatType.LightResist);
    public int DarkResist => statBlock.Get(StatType.DarkResist);


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


    [Header("버프 디버프")]
    public List<StatusEffect> activeEffects = new();
    [SerializeField] protected bool isHidden = false;
    public bool IsHidden
    {
        get => isHidden;
        set
        {
            isHidden = value;
            OnHide();
        }
    }
    public bool isInvincible =false;
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
    protected CharacterAnimator _anim;
    protected SpriteRenderer _spriteRenderer;

    //체력 변경 이벤트
    public event Action OnHealthRatioChanged;
    public event Action OnShieldChanged;
    public event Action OnManaChanged;
    public event Action<DamageInfo> OnPreTakeDamage;
    public event Action<DamageInfo> OnTakeDamage;
    public event Action<DamageInfo> OnAttack; 
    public event Action OnTileChanged;


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

    public void PlaceOnTile(Level level,Tile tile)
    {
        curLevel = level;
        CurTile = tile;
        transform.position = new Vector3(CurTile.gridPosition.x, CurTile.gridPosition.y, 0);
    }
    /// <summary>
    /// 일반 공격의 경우
    /// </summary>
    public virtual void Attack(CharacterStats target, DamageType damageType = DamageType.None, int statusEffectID = -1)
    {
        //기본 데미지 계산
        float baseDamage = UnityEngine.Random.Range(AttackMin, AttackMax);
        //치명타 게산
        bool isCrit = UnityEngine.Random.value < CritChance / 100f;
        if (isCrit)
            Debug.Log("치명타 발생!");

        DamageInfo damageInfo = new DamageInfo(baseDamage, damageType, this, target, isCrit);


        target.TakeDamage(damageInfo);
        Debug.Log($"{gameObject.name}가 {target.gameObject.name}을 공격함니다." + $"속성:{damageType}, 기본 대미지:{baseDamage}");
    }
    /// <summary>
    /// 공격 배율이 있는경우
    /// </summary>
    public virtual void Attack(CharacterStats target, float multiplier, DamageType damageType = DamageType.None, int statusEffectID = -1)
    {
        //기본 데미지 계산
        float baseDamage = UnityEngine.Random.Range(AttackMin, AttackMax) * multiplier;
        //치명타 게산
        bool isCrit = UnityEngine.Random.value < CritChance / 100f;
        if (isCrit)
            Debug.Log("치명타 발생!");

        DamageInfo damageInfo = new DamageInfo(baseDamage, damageType, this, target, isCrit);

        target.TakeDamage(damageInfo);
        Debug.Log($"{gameObject.name}가 {target.gameObject.name}을 공격함니다." + $"속성:{damageType}, 기본 대미지:{baseDamage}");
    }
    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        if(damageInfo.source != null )
        {
            damageInfo.source.OnAttackDamage(damageInfo);
        }
        OnPreTakeDamage?.Invoke(damageInfo);
        float value = DamageCalculator.CalculateDamage(damageInfo);
        if(isInvincible) 
            value = 0;

        if (CurrentShield > 0)
        {
            if (value > CurrentShield)
            {
                Debug.Log($"{gameObject.name}는 쉴드에 {CurrentShield}의 피해를 받았습니다.");
                value -= CurrentShield;
                CurrentShield = 0;
            }
            else
            {
                Debug.Log($"{gameObject.name}는 쉴드에 {value}의 피해를 받았습니다.");
                CurrentShield -= value;
                value = 0;
            }
        }

        CurrentHealth -= value;
        UIManager.ShowOnce<DamageText>(value, transform.position);
        Debug.Log($"{gameObject.name}는 {value}의 피해를 받았습니다.");
        OnTakeDamage?.Invoke(damageInfo);
        _anim.PlayKnockBack();
    }

    public void OnAttackDamage(DamageInfo damageInfo)
    {
        OnAttack?.Invoke(damageInfo);
    }


    public virtual void Heal(float amount)
    {
        float multiplier = RegenerationMultiplier / 100f;
        CurrentHealth += amount * multiplier;
        Debug.Log($"{gameObject.name}는 {amount * multiplier}만큼 회복되었습니다.");
    }

    public void ChangeMana(float amount)
    {
        CurrentMana += amount;
        Debug.Log($"{gameObject.name}는 {amount}만큼 마나가 회복되었습니다.");
    }

    public virtual void GetShield(float amount)
    {
        float multiplier = ShieldMultiplier / 100f;
        CurrentShield += amount * multiplier;
    }

    public virtual void OnHide()
    {
        _spriteRenderer.color = new Color(1f,1f, 1f, IsHidden ? 0.7f: 1f);
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

        OnTileChanged?.Invoke();
    }

    public void ApplyEffect(StatusEffect effect)
    {
        effect.Initialize(TurnManager.Instance.globalTime);
        effect.OnApply(this);
        if (effect.TryRegist(this))
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
            .OfType<IImmunity>()
            .Any(im => im.BlockedTypes.Contains(effectType));
    }
}
