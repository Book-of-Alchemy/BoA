using UnityEngine;
using System.Collections;

public abstract class EnemyController : Character
{
    [SerializeField] protected Transform _player;
    protected EnemyState _state = EnemyState.Patrol;

    protected virtual void Awake()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.AddEnemy(this);
    }

    protected virtual void Start()
    {
        if (_player == null && GameManager.Instance != null)
            _player = GameManager.Instance.PlayerTransform;
    }

    public IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(EnemyTurnRoutine());
    }

    protected abstract IEnumerator EnemyTurnRoutine();
}

public enum EnemyState
{
    Patrol,
    Chase
}