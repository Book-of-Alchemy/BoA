using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    None, Fire, Water, Ice, Electric, Earth, Wind, Light, Dark
}

[System.Serializable]
public class CharacterStats
{
    // 기본 정보
    public int Level = 1;
    public int Exp = 0;
    public int ExpToNextLevel => 100 + (Level - 1) * 20;

    // 전투 능력치
    public int MaxHp;
    public int CurrentHp;

    public int MaxMp;
    public int CurrentMp;

    public int Atk; // 공격력
    public int Def; // 방어력

    public float CritRate;   // 치명타 확률 (ex: 0.2f → 20%)
    public float CritDmg;    // 치명타 배율 (ex: 1.5f → 150%)

    public float DodgeRate;  // 회피율
    public float Accuracy;   // 명중률

    public float MoveSpeed = 5f; // 이동 속도

    public Dictionary<ElementType, float> Resistances = new();

    // 초기화 및 레벨업
    public void Initialize(int level = 1)
    {
        Level = Mathf.Max(1, level);
        MaxHp = 100 + (Level - 1) * 10;
        MaxMp = 50 + (Level - 1) * 10;
        Atk = 20 + (Level - 1) * 10;
        Def = 10 + (Level - 1) * 10;

        CurrentHp = MaxHp;
        CurrentMp = MaxMp;

        CritRate = 0.1f;
        CritDmg = 1.5f;
        DodgeRate = 0.05f;
        Accuracy = 0.95f;
    }

    public void LevelUp()
    {
        Level++;
        MaxHp += 10;
        MaxMp += 10;
        Atk += 10;
        Def += 10;

        CurrentHp = MaxHp;
        CurrentMp = MaxMp;
        Exp = 0;
    }

    public bool GainExp(int amount)
    {
        Exp += amount;
        if (Exp >= ExpToNextLevel)
        {
            LevelUp();
            return true;
        }
        return false;
    }
}