using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

public class TurnManager : Singleton<TurnManager>
{
    public List<UnitBase> allUnits = new List<UnitBase>();
    public int globalTime = 0;
    public float turnSpeed = 5;//나눈 값을 기준으로 정함 5 = 0.2초 10 = 0.1초 향후 이걸 기준으로 가속 + 도트윈 + 애니메이션에도 적용
    public void StartTurnCycle()
    {
        StartCoroutine(TickLoop());
    }

    private IEnumerator TickLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(1/ turnSpeed);//애니메이션 처리를 위해 적용 향후 이걸 기준으로 가속 + 도트윈 + 애니메이션에도 적용
        while (allUnits.Count > 0)
        {
            allUnits.Sort((a, b) => ComparePlayer(a, b));//플레이어를 맨앞으로 같은 턴일시 플레이어를 우선
            // 모든 유닛 버프 디버프
            foreach (var unit in allUnits.ToArray())
            {
                unit.Stats?.TickEffects(globalTime);
            }

            foreach (var unit in allUnits.ToArray()) // 시행중에 add remove발생시 에러 발생 복사한 배열사용
            {
                if (unit.nextActionTime <= globalTime)
                {
                    unit.StartTurn();
                    if (unit is PlayerUnit)
                    {
                        PlayerUnit playerUnit = unit as PlayerUnit;
                        yield return new WaitUntil(() => !playerUnit.IsWaitingForInput);
                    }

                    int cost = unit.GetModifiedActionCost();
                    Debug.Log($"[Tick {globalTime}] {unit.name} 턴 시작 (cost: {cost})");
                    unit.nextActionTime += cost;
                        yield return wait;
                }
            }



            globalTime++;
            yield return null;
        }
    }
    private int ComparePlayer(UnitBase a, UnitBase b)
    {
        //int result = a.nextActionTime.CompareTo(b.nextActionTime);
        //if (result != 0) return result;
        if (a.IsPlayer && !b.IsPlayer) return -1;
        if (!a.IsPlayer && b.IsPlayer) return 1;
        return 0;
    }
    public void AddUnit(UnitBase unit)
    {
        unit.Init();
    }
    public void RemoveUnit(UnitBase unit) => allUnits.Remove(unit);

    /*public static TurnManager Instance { get; private set; }

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
    }*/
}
