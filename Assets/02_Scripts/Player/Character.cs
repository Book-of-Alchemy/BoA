using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int currentHP = 100;
    protected bool isMoving = false;

    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        gameObject.SetActive(false);
    }

    protected IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        TurnManager.Instance.EndTurn();
    }
}
