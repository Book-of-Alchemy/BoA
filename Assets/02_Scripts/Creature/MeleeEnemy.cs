using UnityEngine;
using System.Collections;

public class MeleeEnemy : EnemyController
{
    [SerializeField] private int _detectionRange = 5;

    protected override IEnumerator EnemyTurnRoutine()
    {
        if (_player == null || Stats.CurrentHp <= 0)
            yield break;

        Vector3Int playerCell = Vector3Int.FloorToInt(_player.position);
        Vector3Int selfCell = Vector3Int.FloorToInt(transform.position);

        int dist = Mathf.Abs(playerCell.x - selfCell.x) + Mathf.Abs(playerCell.y - selfCell.y);

        if (dist == 1)
        {
            Character target = _player.GetComponent<Character>();
            if (target != null)
            {
                Attack(target);
                yield return new WaitForSeconds(0.2f);
                yield break;
            }
        }

        if (dist <= _detectionRange)
        {
            Vector3 dir = (_player.position - transform.position).normalized;
            Vector2Int moveDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
                ? new Vector2Int((int)Mathf.Sign(dir.x), 0)
                : new Vector2Int(0, (int)Mathf.Sign(dir.y));

            yield return StartCoroutine(Move(moveDir));
        }
        else
        {
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            Vector2Int randDir = dirs[Random.Range(0, dirs.Length)];
            yield return StartCoroutine(Move(randDir));
        }
    }
}
