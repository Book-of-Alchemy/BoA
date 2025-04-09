using UnityEngine;

public class PlayerController : Character
{
    void Update()
    {
        if (TurnManager.Instance.currentTurn != TurnManager.Turn.Player || isMoving)
            return;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input != Vector2.zero)
        {
            Vector3 targetPos = transform.position + new Vector3(input.x, input.y, 0);
            StartCoroutine(Move(targetPos));
        }
    }
}