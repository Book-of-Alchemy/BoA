using UnityEngine;

public class PlayerStats : CharacterStats
{
    // BuffManager를 통해 상태 효과를 관리할 수 있도록 프로퍼티 추가
    public BuffManager BuffManager { get; private set; }

    void Awake()
    {
        // BuffManager 컴포넌트가 없으면 추가
        BuffManager = GetComponent<BuffManager>();
        if (BuffManager == null)
            BuffManager = gameObject.AddComponent<BuffManager>();

        // GameManager에 플레이어 등록
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
