using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public enum Turn { Player, Enemy }
    public Turn CurrentTurn { get; private set; } = Turn.Player;

    [SerializeField] private List<EnemyController> _enemies = new List<EnemyController>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void EndTurn()
    {
        if (CurrentTurn == Turn.Player)
        {
            CurrentTurn = Turn.Enemy;
            StartCoroutine(EnemyTurnCycle());
        }
        else
        {
            CurrentTurn = Turn.Player;
        }
    }

    private IEnumerator EnemyTurnCycle()
    {
        Debug.Log("적 턴 사이클 시작");
        foreach (EnemyController enemy in _enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy || enemy.Stats.CurrentHp <= 0)
                continue;

            enemy.Act();
            Debug.Log("적 행동 시작");
            while (enemy.IsMoving)
                yield return null;

            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("모든 적 턴 종료됨");
        TurnManager.Instance.EndTurn();
    }

    public void AddEnemy(EnemyController enemy)
    {
        if (!_enemies.Contains(enemy))
            _enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        if (_enemies.Contains(enemy))
            _enemies.Remove(enemy);
    }

    public void SetEnemies(List<EnemyController> enemies)
    {
        _enemies = enemies;
    }
}