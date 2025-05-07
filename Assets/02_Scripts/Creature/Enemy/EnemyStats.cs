using UnityEngine;

public class EnemyStats : CharacterStats, IPoolableId
{
    [Header("드랍 아이템 설정")]
    [Tooltip("죽었을 때 드랍할 아이템 데이터")]
    public ItemData dropItemData;

    [Tooltip("드랍할 아이템 수량")]
    public int dropAmount = 1;

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
        Debug.Log("적이 사망했습니다.");
        TryDropItem();

        TurnManager.Instance.RemoveUnit(unitBase);

        StopAllCoroutines();

        Invoke(nameof(DelayDestroy), 0.1f);
    }

    private void TryDropItem()
    {
        if (dropItemData == null || dropAmount <= 0)
            return;

        var prefab = dropItemData.itemPrefab;
        if (prefab == null)
        {
            Debug.LogError($"[{name}] 아아템 데이터에 프리팹이 할당되지 않음");
            return;
        }

        for (int i = 0; i < dropAmount; i++)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
            var baseItem = go.GetComponent<BaseItem>();
            if (baseItem != null)
                baseItem.DropItem(dropItemData, 1, this.CurTile);
        }
    }
    private void DelayDestroy()
    {
        Destroy(gameObject);
    }
}
