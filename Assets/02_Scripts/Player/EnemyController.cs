using UnityEngine;
using System.Collections;

public class EnemyController : Character
{
    private bool hasActed = false;

    void Update()
    {
        if (TurnManager.Instance.currentTurn == TurnManager.Turn.Enemy && !isMoving && !hasActed)
        {
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    IEnumerator EnemyTurnRoutine()
    {
        hasActed = true;

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 moveDir = directions[Random.Range(0, directions.Length)];
        Vector3 targetPos = transform.position + new Vector3(moveDir.x, moveDir.y, 0);

        yield return StartCoroutine(Move(targetPos));

        hasActed = false;
    }
}
