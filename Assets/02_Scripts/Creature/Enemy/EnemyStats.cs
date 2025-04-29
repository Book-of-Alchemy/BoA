using UnityEngine;

public class EnemyStats : CharacterStats,IPoolableId
{
    [SerializeField]
    private int id;
    public int Id { get => id; set => id = value; }

    protected override void Awake()
    {
        base.Awake();
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
