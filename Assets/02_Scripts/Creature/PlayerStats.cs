using UnityEngine;

public class PlayerStats : CharacterStats
{
    public void GainExperience(int exp)
    {
        Experience += exp;
        Debug.Log(exp + "의 경험치 획득");

        // 레벨업 조건 확인 후 LevelUp 호출 가능
    }

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
        Debug.Log("레벨업 " + Level);
    }
}
