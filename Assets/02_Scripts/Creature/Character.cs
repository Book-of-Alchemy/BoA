using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed = 5f;
    [SerializeField] protected int _currentHp = 100;
    protected bool _isMoving;

    public bool IsMoving => _isMoving;

    public virtual void TakeDamage(int dmg)
    {
        _currentHp -= dmg;
        if (_currentHp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        gameObject.SetActive(false);
    }

    protected IEnumerator Move(Vector3 targetPos)
    {
        _isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        _isMoving = false;

        TurnManager.Instance.EndTurn();
    }
}