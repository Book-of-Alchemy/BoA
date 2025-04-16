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

    private List<EnemyStats> enemies;
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
            Player = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
        else
            Debug.LogWarning("GameManager에 플레이어가 등록되지 않았습니다.");

        enemies = GameManager.Instance.Enemies;
        CurrentTurn = TurnState.PlayerTurn;
    }

    void Update()
    {
        // 플레이어 턴 종료 조건: 행동력이 0 이하일 때
        if (CurrentTurn == TurnState.PlayerTurn && Player.BuffManager.GetFinalActionPoints() <= 0 && !_isEnemyTurnRunning)
        {
            // 플레이어의 상태 효과 업데이트 후 적 턴 실행
            Player.BuffManager.UpdateBuffs();
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        _isEnemyTurnRunning = true;
        CurrentTurn = TurnState.EnemyTurn;

        // 적 턴 시작 전 각 적의 버프 업데이트
        foreach (EnemyStats enemy in enemies.ToArray())
        {
            if (enemy == null)
                continue;
            enemy.BuffManager.UpdateBuffs();
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
                yield return StartCoroutine(enemyController.TakeTurn());
            else
            {
                Debug.Log(enemy.gameObject.name + " takes turn (dummy)");
                yield return new WaitForSeconds(0.5f);
            }
        }

        // 적 턴 종료 후 플레이어 AP 복구 (버프 업데이트)
        Player.BuffManager.ApplyBuff(0, 0);  // 효과 적용 없이 재계산
        CurrentTurn = TurnState.PlayerTurn;
        _isEnemyTurnRunning = false;
        Debug.Log("복구된 행동력: " + Player.BuffManager.GetFinalActionPoints());
        yield break;
    }
}
