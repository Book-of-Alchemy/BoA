using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnState CurrentTurn;
    public PlayerStats Player;

    // GameManager에서 가져온 리스트를 사용
    private List<EnemyStats> _enemies;

    private bool _isEnemyTurnRunning = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (GameManager.Instance.PlayerTransform != null)
        {
            Player = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogWarning("GameManager에 플레이어가 등록되지 않았습니다.");
        }

        // GameManager의 Enemies 리스트를 사용
        _enemies = GameManager.Instance.Enemies;

        CurrentTurn = TurnState.PlayerTurn;
    }

    void Update()
    {
        if (CurrentTurn == TurnState.PlayerTurn && Player.ActionPoints <= 0 && !_isEnemyTurnRunning)
        {
            StartCoroutine(EnemyTurn());
        }
    }

    // 적 턴 진행 코루틴
    IEnumerator EnemyTurn()
    {
        _isEnemyTurnRunning = true;
        CurrentTurn = TurnState.EnemyTurn;

        // GameManager에서 등록된 적 리스트를 안전하게 순회
        foreach (EnemyStats enemy in _enemies.ToArray())
        {
            if (enemy == null)
                continue;
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
                yield return StartCoroutine(enemyController.TakeTurn());
            else
            {
                Debug.Log(enemy.gameObject.name + " takes turn (dummy)");
                yield return new WaitForSeconds(0.5f);
            }
        }

        Player.ActionPoints = 1.0f;
        CurrentTurn = TurnState.PlayerTurn;
        _isEnemyTurnRunning = false;
        Debug.Log("복구된 행동력: " + Player.ActionPoints);
        yield break;
    }
}
