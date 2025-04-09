using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public enum Turn { Player, Enemy }
    public Turn CurrentTurn { get; private set; } = Turn.Player;

    [SerializeField] private List<EnemyController> _enemies = new List<EnemyController>();
    private bool _isProcessingTurn = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void EndTurn()
    {
        if (_isProcessingTurn) return;

        if (CurrentTurn == Turn.Player)
        {
            CurrentTurn = Turn.Enemy;
            StartCoroutine(EnemyTurnRoutine());
        }
        else
        {
            CurrentTurn = Turn.Player;
        }
    }

    private IEnumerator EnemyTurnRoutine()
    {
        _isProcessingTurn = true;

        foreach (EnemyController enemy in _enemies)
        {
            yield return enemy.ExecuteTurn();
            yield return new WaitForSeconds(0.1f);
        }

        CurrentTurn = Turn.Player;
        _isProcessingTurn = false;
    }

    public void AddEnemy(EnemyController enemy)
    {
        if (!_enemies.Contains(enemy))
            _enemies.Add(enemy);
    }

    public void SetEnemies(List<EnemyController> enemies)
    {
        _enemies = enemies;
    }
}
