using UnityEngine;

public class EnemyStats : CharacterStats
{
    // 적 전용 추가 기능 구현 가능 (예: 죽음 처리)

    /// <summary>
    /// 적 사망 후 처리  
    /// 아이템 드랍, 경험치 분배 등 추가 로직 가능
    /// </summary>
    public void OnDeath()
    {
        Debug.Log(gameObject.name + " has been defeated.");
    }
}
