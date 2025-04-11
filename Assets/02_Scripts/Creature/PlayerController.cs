using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : Character
{
    private Vector2Int _lastMoveDir;
    private bool _inputLocked;//추가입력방지

    private void Start()
    {
        GameManager.Instance.RegisterPlayer(transform);
    }

    public void OnMove(InputValue value)
    {
        if (_inputLocked || TurnManager.Instance.CurrentTurn != TurnManager.Turn.Player || IsMoving)
            return;

        Vector2 input = value.Get<Vector2>();

        // 대각선 이동 허용 처리
        input.x = Mathf.Abs(input.x) > 0.1f ? Mathf.Sign(input.x) : 0;
        input.y = Mathf.Abs(input.y) > 0.1f ? Mathf.Sign(input.y) : 0;

        Vector2Int moveDir = new Vector2Int((int)input.x, (int)input.y);
        if (moveDir == Vector2Int.zero)
            return;

        _lastMoveDir = moveDir;
        _inputLocked = true;
        StartCoroutine(MoveWithUnlock(moveDir));
        TurnManager.Instance.EndTurn();
    }

    private IEnumerator MoveWithUnlock(Vector2Int dir)
    {
        yield return StartCoroutine(Move(dir));
        _inputLocked = false;
    }

    public void OnInteract()
    {
        Vector2Int dir = _lastMoveDir;
        Vector3Int currentCell = Vector3Int.FloorToInt(transform.position);
        Vector3Int targetCell = currentCell + new Vector3Int(dir.x, dir.y, 0);

        Collider2D hit = Physics2D.OverlapPoint(targetCell + new Vector3(0.5f, 0.5f));
        if (hit != null)
        {
            Character enemy = hit.GetComponent<Character>();
            if (enemy != null && enemy != this)
            {
                Attack(enemy);
                TurnManager.Instance.EndTurn();
            }
        }
    }

    public void OnCancel() => Debug.Log("Cancel 키 입력됨");
    public void OnMenu() => Debug.Log("Menu 키 입력됨");
    public void OnDash() => Debug.Log("Dash 키 입력됨");
}