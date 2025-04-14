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

    // 현재 턴 상태 (플레이어 혹은 적)
    public TurnState CurrentTurn;

    // 씬 내 플레이어 스탯 컴포넌트 참조
    public PlayerStats Player;

    // 씬 내 모든 적 스탯 컴포넌트 참조 리스트
    private List<EnemyStats> _enemies;

    // 적 턴 진행 중 여부
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
        // 플레이어 스탯 컴포넌트가 할당되지 않았다면 씬에서 찾음
        if (Player == null)
            Player = FindObjectOfType<PlayerStats>();

        // EnemyStats 컴포넌트가 붙은 모든 오브젝트 찾아 리스트에 저장
        _enemies = new List<EnemyStats>(FindObjectsOfType<EnemyStats>());

        // 초기 턴 상태 설정
        CurrentTurn = TurnState.PlayerTurn;
    }

    void Update()
    {
        // 플레이어 턴일 때 ActionPoints가 0 이하이면 적 턴 시작
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

        // 각 적들이 생성된 순서대로 턴 진행
        foreach (EnemyStats enemy in _enemies)
        {
            // EnemyController 컴포넌트가 붙어 있다면 TakeTurn 코루틴 호출
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
                yield return StartCoroutine(enemyController.TakeTurn());
            else
            {
                Debug.Log(enemy.gameObject.name + " takes turn (dummy)");
                yield return new WaitForSeconds(0.5f); // 간단한 대기 처리
            }
        }

        // 적 턴 종료 후 플레이어 ActionPoints 복구
        Player.ActionPoints = 1.0f;
        CurrentTurn = TurnState.PlayerTurn;
        _isEnemyTurnRunning = false;
        Debug.Log("복구된 행동력: " + Player.ActionPoints);
        yield break;
    }
}
