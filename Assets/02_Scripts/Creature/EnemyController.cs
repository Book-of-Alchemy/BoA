using UnityEngine;
using System.Collections;

public abstract class EnemyController : Character
{
    [SerializeField] protected Transform _player;

    protected virtual void Start()
    {
        if (_player == null && GameManager.Instance != null)
            _player = GameManager.Instance.PlayerTransform;
    }

    public virtual void Act()
    {
        StartCoroutine(ExecuteTurn());
    }

    private IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(EnemyTurnRoutine());
    }

    protected abstract IEnumerator EnemyTurnRoutine();

    protected override void Die()
    {
        base.Die();
        TurnManager.Instance.RemoveEnemy(this);
    }
}

public enum EnemyState
{
    Patrol,
    Chase
}