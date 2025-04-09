using UnityEngine;
using System.Collections;

public class RangeEnemy : EnemyController
{
    [SerializeField] private float _attackRange = 4f;

    protected override IEnumerator EnemyTurnRoutine()
    {
        float tileDistance = Mathf.Abs(_player.position.x - transform.position.x) + Mathf.Abs(_player.position.y - transform.position.y);

        if (tileDistance <= _attackRange)
        {
            Debug.Log("원거리 공격!");
            //애니메이션 및 투사체 발사 구현
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            Vector3 dir = (_player.position - transform.position).normalized;
            Vector2 moveDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
                ? new Vector2(Mathf.Sign(dir.x), 0)
                : new Vector2(0, Mathf.Sign(dir.y));
            Vector3 targetPos = transform.position + new Vector3(moveDir.x, moveDir.y, 0);
            yield return StartCoroutine(Move(targetPos));
        }
    }
}