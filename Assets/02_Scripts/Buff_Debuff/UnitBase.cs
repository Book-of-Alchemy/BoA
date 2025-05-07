using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{

    public int currentTime = 0;
    public int nextActionTime = 0;
    public StatEntry actionCostStat = new StatEntry() { baseValue = 10 };
    public int actionCost => actionCostStat.Value;
    public float actionCostMultiplier = 1f;
    public bool ActionInProgress { get; private set; }
    public Tile CurTile => Stats?.CurTile;
    

    public CharacterStats Stats;
    private int? _totalCost = null;

    protected virtual void Awake()
    {
        Stats = GetComponent<CharacterStats>();
    }

    public virtual void Init()
    {
        currentTime = TurnManager.Instance.globalTime;
        nextActionTime = currentTime + actionCost;
    }
    public abstract void PerformAction();
    public void SetNextActionCost(int cost)
    {
        _totalCost = cost;
    }
    public virtual int GetModifiedActionCost()
    {
        if (_totalCost.HasValue)
        {
            int cost = _totalCost.Value;
            _totalCost = null;            // 한 번 쓰고 자동 초기화
            return cost;
        }
        return Mathf.Max(1, Mathf.RoundToInt(actionCost * actionCostMultiplier));
    }

    public void StartTurn()
    {
        ActionInProgress= true;
        PerformAction();
    }

    public void AdvanceTime(int cost)
    {
        currentTime = nextActionTime;
        nextActionTime += cost;
    }

    public void OnTurnEnd()
    {
        if (ActionInProgress)
            ActionInProgress = false;
    }

    public bool IsPlayer => this is PlayerUnit;


    
}
