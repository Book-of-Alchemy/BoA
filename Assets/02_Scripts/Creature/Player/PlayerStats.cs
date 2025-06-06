using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private PlayerController _player;
    public List<Artifact> equipArtifacts = new();
    public bool isManaOverload = false;
    public bool isEasyInstallationKit = false;
    public bool isMarksman = false;
    public bool isPrecisionAim = false;
    private bool isDead = false;
    //다음레벨까지 필요한 경험치(내부 계산용)
    private int _nextLevelExp => Mathf.FloorToInt(10f * Mathf.Pow(level, 1.5f));
    //다음레벨까지 필요한 경험치(읽기 전용)
    public int nextLevelExp => _nextLevelExp;

    public event Action OnExperienceChanged;
    public event Action<int> OnLevelChanged;

    public override Tile CurTile
    {
        get => curTile;
        set
        {
            TurnOffVision();
            curTile = value;
            TurnOnVision();
        }
    }
    protected override void Awake()
    {
        base.Awake();
        // GameManager에 플레이어 등록
        GameManager.Instance.RegisterPlayer(this);
        _player = GetComponent<PlayerController>();
        ResearchSystem.Instance.ApplyModifiersTo(this);
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
    }

    public void GainExperience(int exp)
    {
        experience += exp;
        OnExperienceChanged?.Invoke();

        // 레벨업 체크
        while (experience >= _nextLevelExp)
            LevelUp();
    }

    public void LevelUp()
    {
        experience -= _nextLevelExp;
        level++;
        statBlock.SetBaseValue(StatType.MaxHealth, 100 + 10 * level);
        CurrentHealth = MaxHealth;
        statBlock.SetBaseValue(StatType.MaxMana, 50 + 5 * level);
        CurrentMana = MaxMana;
        statBlock.SetBaseValue(StatType.Attack, 10 + 1 * level);
        OnExperienceChanged?.Invoke();
        OnLevelChanged?.Invoke(level);
        if (level < 62)
            UIManager.Show<UI_LvSelect>();
        else
            UIManager.Show<UI_Text>("유물을 획득하지 못할 것 같다...");
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
    }

    public override void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;
        base.Die();
        UIManager.Show<UI_DungeonResult>();
        TurnManager.Instance.StopTurnCycle();
        _player.OnDisable();
    }


    void TurnOffVision()
    {
        if (curTile == null || curLevel == null)
            return;

        foreach (var tile in tilesOnVision)
        {
            if (tile == null) continue;

            tile.IsOnSight = false;
        }

    }

    void TurnOnVision()
    {
        if (curTile == null || curLevel == null)
            return;

        foreach (var tile in tilesOnVision)
        {
            if (tile == null) continue;

            tile.IsOnSight = true;
            tile.IsExplored = true;
        }

    }

}
