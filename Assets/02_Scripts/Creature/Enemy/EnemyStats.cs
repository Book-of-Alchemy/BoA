using UnityEngine;

public class EnemyStats : CharacterStats
{
    void Start()
    {
        // 이 적을 GameManager에 등록합니다.
        GameManager.Instance.RegisterEnemy(this);
    }

    void OnDestroy()
    {
        // 파괴될 때 GameManager에서 제거합니다.
        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterEnemy(this);
    }
    public override void Die()
    {
        Destroy(gameObject);
    }
}
