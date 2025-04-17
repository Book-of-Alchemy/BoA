using UnityEngine;

public class EnemyStats : CharacterStats
{
    public BuffManager BuffManager { get; private set; }

    void Awake()
    {
        BuffManager = GetComponent<BuffManager>();
        if (BuffManager == null)
            BuffManager = gameObject.AddComponent<BuffManager>();

        // GameManager에 적 등록
        GameManager.Instance.RegisterEnemy(this);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterEnemy(this);
    }
    public override void Die()
    {
        Destroy(gameObject);
    }
}
