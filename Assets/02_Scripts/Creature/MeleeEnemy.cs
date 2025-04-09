using UnityEngine;
using System.Collections;

public class MeleeEnemy : EnemyController
{
    [SerializeField] private float _detectionRange = 5f;

    protected override IEnumerator EnemyTurnRoutine()
    {
        float tileDistance = Mathf.Abs(_player.position.x - transform.position.x) + Mathf.Abs(_player.position.y - transform.position.y);
        Vector3 targetPos = transform.position;

        if (tileDistance <= _detectionRange)
        {
            Vector3 dir = (_player.position - transform.position).normalized;
            Vector2 moveDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
                ? new Vector2(Mathf.Sign(dir.x), 0)
                : new Vector2(0, Mathf.Sign(dir.y));
            targetPos += new Vector3(moveDir.x, moveDir.y, 0);
        }
        else
        {
            Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            Vector2 randDir = dirs[Random.Range(0, dirs.Length)];
            targetPos += new Vector3(randDir.x, randDir.y, 0);
        }

        yield return StartCoroutine(Move(targetPos));
    }
}
