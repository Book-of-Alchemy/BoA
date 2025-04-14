using UnityEngine;

public class PlayerStats : CharacterStats
{
    void Awake()
    {
        // 씬에서 생성된 플레이어는 GameManager에 등록
        GameManager.Instance.RegisterPlayer(transform);
    }
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
