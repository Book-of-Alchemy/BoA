using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnManager : Singleton<TurnManager>
{
    public List<UnitBase> allUnits = new List<UnitBase>();
    public List<TileEffect> allTileEffects = new List<TileEffect>();
    HashSet<int> unitIds = new HashSet<int>();

    // 글로벌 시간 변경 이벤트 추가
    public event Action OnGlobalTimeChanged;

    [SerializeField] private int _globalTime = 0;
    public int globalTime
    {
        get => _globalTime;
        private set
        {
            _globalTime = value;
            OnGlobalTimeChanged?.Invoke();
        }
    }

    public float turnSpeed = 10;//나눈 값을 기준으로 정함 5 = 0.2초 10 = 0.1초 향후 이걸 기준으로 가속 + 도트윈 + 애니메이션에도 적용
    [SerializeField] private float originSpeed = 10;

    [SerializeField] private UnitBase curUnit;



    //void OnEnable()
    //{
    //    GameSceneManager.Instance.OnSceneTypeChanged += OnSceneLoaded;
    //}

    //void OnDisable()
    //{
    //    GameSceneManager.Instance.OnSceneTypeChanged -= OnSceneLoaded;
    //}

    //public void OnSceneLoaded(SceneType sceneType)
    //{
    //    ResetManager();
    //    if(sceneType == SceneType.Dungeon)
    //        StartTurnCycle();   
    //}

    public void StartTurnCycle()
    {
        AddUnit(GameManager.Instance.PlayerTransform.GetComponent<PlayerUnit>());
        StartCoroutine(TickLoop());
    }

    private IEnumerator TickLoop()
    {
        //WaitForSeconds wait = new WaitForSeconds(1f / turnSpeed);

        while (allUnits.Count > 0)
        {
            // 매 턴 시작할 때마다 파괴된유닛 모두 제거
            allUnits.RemoveAll(u => u == null || !u.gameObject.activeInHierarchy);

            if (allUnits.Count == 0)
                yield break;

            allUnits.Sort((a, b) => ComparePlayer(a, b));

            // 버프/디버프 처리 + visual 처리
            foreach (var unit in allUnits.ToArray())
            {
                if (unit == null) continue;// null 체크
                if (!unit.gameObject.activeSelf)
                {
                    RemoveUnit(unit);
                    continue;
                }
                unit.Stats?.TickEffects(globalTime);
                if (unit is EnemyUnit enemyUnit)
                    enemyUnit.UpdateVisual();
            }

            foreach (var effect in allTileEffects.ToArray())
            {
                if (effect == null) continue;
                if (!effect.gameObject.activeSelf)
                {
                    RemoveTileEffectt(effect);
                    continue;
                }
                if (effect.NextActionTime <= globalTime)
                {
                    effect.StartTurn();
                    effect.NextActionTime += effect.ActionCost;
                    effect.OnTurnEnd();
                }
            }

            // 실제 턴 처리
            foreach (var unit in allUnits.ToArray())
            {
                if (unit == null || !unit.gameObject.activeSelf)
                    continue;


                if (unit.NextActionTime <= globalTime)
                {
                    curUnit = unit;
                    if (!unit.IsPlayer && unit.CurTile != null)// 시야에 없는적 애니메이션 속도 가속
                    {
                        if (!unit.CurTile.IsOnSight)//null check 책임 분리
                            turnSpeed = 200;
                    }

                    unit.StartTurn();

                    if (unit is PlayerUnit playerUnit)
                    {
                        yield return new WaitUntil(() => !playerUnit.IsWaitingForInput);
                    }

                    int cost = unit.GetModifiedActionCost();

                    unit.NextActionTime += cost;

                    turnSpeed = originSpeed;
                    //yield return wait;
                    //if (!unit.IsPlayer)//일단 enemy만 로직적으로 대기처리 향후 player역시 0.1f대기가 아닌 로직적 대기 현재 player 턴처리문제로 enemy가 인접해있을때 못따라오는 현상 종종발생
                    yield return new WaitUntil(() => !unit.ActionInProgress || unit == null || !unit.gameObject.activeSelf);
                    //else
                    //{
                    UpdateAllUnitVisual();
                    //yield return null;
                    //}
                }
            }
            TileUtility.RefreshLevelSight();
            globalTime++;
            yield return null;
        }
    }

    void UpdateAllUnitVisual()
    {
        foreach (var unit in allUnits.ToArray())
        {
            if (unit == null) continue;// 비주얼 후처리
            if (unit is EnemyUnit enemyUnit)
                enemyUnit.UpdateVisual();
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
        if (!unitIds.Add(unit.GetInstanceID()))
            return;
        allUnits.Add(unit);
        unit.Init();
    }
    public void RemoveUnit(UnitBase unit)
    {
        if (unitIds == null) unitIds = new HashSet<int>();
        if (allUnits == null) allUnits = new List<UnitBase>();

        unitIds.Remove(unit.GetInstanceID());
        allUnits.Remove(unit);
    }

    public void RemoveAllEnemy()
    {
        foreach (var unit in allUnits.ToArray())
        {
            if (unit.IsPlayer) continue;
            unitIds.Remove(unit.GetInstanceID());
            allUnits.Remove(unit);
        }
    }
    public void AddTileEffect(TileEffect tileEffect)
    {
        if (!unitIds.Add(tileEffect.GetInstanceID()))
            return;
        allTileEffects.Add(tileEffect);
    }

    public void RemoveTileEffectt(TileEffect tileEffect)
    {
        if (unitIds == null) unitIds = new HashSet<int>();
        if (allUnits == null) allUnits = new List<UnitBase>();

        unitIds.Remove(tileEffect.GetInstanceID());
        allTileEffects.Remove(tileEffect);
    }

    public void ResetManager()
    {
        // 현재 진행 중인 코루틴 중단
        StopAllCoroutines();

        // 현재 유닛, 타일 이펙트 목록 초기화
        foreach (var unit in allUnits.ToArray())
        {
            if (unit != null)
                Destroy(unit.gameObject);
        }

        foreach (var effect in allTileEffects.ToArray())
        {
            if (effect != null)
                Destroy(effect.gameObject);
        }

        allUnits.Clear();
        allTileEffects.Clear();
        unitIds.Clear();

        _globalTime = 0;
    }
}
