using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : CharacterStats, IPoolableId
{
    [Header("드랍 아이템 설정")]
    [Tooltip("죽었을 때 드랍할 아이템 데이터")]
    public ItemData dropItemData;

    [Tooltip("드랍할 아이템 수량")]
    public int dropAmount = 1;

    [Tooltip("레벨당 추가 경험치")]
    public int expPerLevel = 5;

    [SerializeField]
    private int id;
    public int Id { get => id; set => id = value; }

    public bool isDead = false;
    public event Action OnDead;
    //protected override void Awake()
    //{
    //    base.Awake();
    //    // GameManager에 적 등록
    //    GameManager.Instance.RegisterEnemy(this);
    //}

    //void OnDestroy()
    //{
    //    if (GameManager.Instance != null)
    //        GameManager.Instance.UnregisterEnemy(this);
    //}
    void OnEnable()
    {
        // 풀에서 재사용될 때 상태 초기화
        isDead = false;
        currentHealth = MaxHealth; 
    }
    public override void Die()
    {
        // 이미 사망 처리되었으면 중복 실행 방지
        if (isDead)
        {
            return;
        }
        isDead = true; // 사망 처리 시작
        OnDead?.Invoke();
        MonsterEvents.RaiseKill(id);

        TryDropItem();
        GiveExpToPlayer();
        _anim.PlayDeath();
        if (CurTile != null)
        {
            CurTile.CharacterStatsOnTile = null;
        }

        if (unitBase != null && TurnManager.Instance != null)
        {
            TurnManager.Instance.RemoveUnit(unitBase);
        }
        
        StopAllCoroutines(); // 모든 코루틴 중지
        StartCoroutine(DelayedReturn());
        
    }
    IEnumerator DelayedReturn()
    {
        yield return new WaitForSeconds(1f);

        EnemyFactory.Instance.enemyPool.ReturnToPool(this);
    }
    
    private void GiveExpToPlayer()
    {
        var player = GameManager.Instance.PlayerTransform;
        if (player != null)
        {
            // 몬스터 레벨에 따른 경험치 계산
            int finalExp = (level * expPerLevel);

            player.GainExperience(finalExp);
        }
    }

    private void TryDropItem()
    {
        if (dropItemData == null || dropAmount <= 0)
            return;

        var prefab = dropItemData.itemPrefab;
        if (prefab == null)
        {
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
}
