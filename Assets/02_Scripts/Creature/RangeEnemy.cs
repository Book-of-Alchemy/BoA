using System.Collections;
using UnityEngine;

public class RangeEnemy : EnemyController
{
    [SerializeField] private float _attackRange = 4f;
    [SerializeField] private float _detectionRange = 6f;

    protected override IEnumerator EnemyTurnRoutine()
    {
        if (_player == null || Stats.CurrentHp <= 0)
            yield break;

        float tileDistance = Mathf.Abs(_player.position.x - transform.position.x) +
                             Mathf.Abs(_player.position.y - transform.position.y);

        if (tileDistance <= _attackRange)
        {
            Debug.Log("플레이어에게 원거리 공격!");
            // 투사체 생성 및 발사 처리
            yield return new WaitForSeconds(0.5f);
        }
        else if (tileDistance <= _detectionRange)
        {
            // 추적 이동
            Vector3 dir = (_player.position - transform.position).normalized;
            Vector2Int moveDir = new Vector2Int((int)Mathf.Sign(dir.x), (int)Mathf.Sign(dir.y));
            yield return StartCoroutine(Move(moveDir));
        }
        else
        {
            // 랜덤 이동
            Vector2Int[] dirs = {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 1), new Vector2Int(-1, 1),
                new Vector2Int(-1, -1), new Vector2Int(1, -1)
            };
            Vector2Int randDir = dirs[Random.Range(0, dirs.Length)];
            yield return StartCoroutine(Move(randDir));
        }
    }
}
