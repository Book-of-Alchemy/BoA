using UnityEngine;

public class PlayerStats : CharacterStats
{
    // 플레이어 전용 기능 구현 가능
    // 예) 경험치 획득, 레벨업 조건 체크 등

    /// <summary>
    /// 경험치 획득  
    /// </summary>
    /// <param name="exp">획득할 경험치</param>
    public void GainExperience(int exp)
    {
        Experience += exp;
        Debug.Log("Player gained " + exp + " experience.");

        // 레벨업 조건 확인 후 LevelUp 호출 가능
    }

    /// <summary>
    /// 레벨업 처리 (예시: 레벨업 시 스탯 증가)
    /// </summary>
    public void LevelUp()
    {
        Level++;
        MaxHealth += 10f;
        CurrentHealth = MaxHealth;
        MaxMana += 5f;
        CurrentMana = MaxMana;
        AttackMin += 1f;
        AttackMax += 1f;
        Defense += 1f;
        Debug.Log("Player leveled up to level " + Level);
    }
}
