using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private PlayerController _player;
    public List<Artifact> equipArtifacts = new();
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
        statBlock.SetBaseValue(StatType.MaxHealth, 100 + 10 * level);
        CurrentHealth = MaxHealth;
        statBlock.SetBaseValue(StatType.MaxMana, 50 + 5 * level);
        CurrentMana = MaxMana;
        statBlock.SetBaseValue(StatType.Attack, 10 + 1 * level);
        Debug.Log("레벨업 " + level);
    }



    public override void Heal(float amount)
    {
        base.Heal(amount);
    }

    public override void Die()
    {
        base.Die();
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
