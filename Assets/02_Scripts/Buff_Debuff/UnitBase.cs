using System.Collections.Generic;
using UnityEngine;

public interface ITurnProcessor
{
    public int CurrentTime { get; set; }
    public int NextActionTime {  get; set; }
    int ActionCost { get; }
    public bool ActionInProgress { get; }
    public Tile CurTile { get; }
    public void PerformAction();

    public void StartTurn();

    public void AdvanceTime(int cost);

    public void OnTurnEnd();

}

public abstract class UnitBase : MonoBehaviour, ITurnProcessor
{
    protected int currentTime = 0;
    public int CurrentTime { get =>  currentTime; set => currentTime = value; }
    protected int nextActionTime = 0;
    public int NextActionTime { get => nextActionTime; set => nextActionTime = value; }
    protected StatEntry actionCostStat = new StatEntry() { baseValue = 10 };
    public StatEntry ActionCostStat { get => actionCostStat; set => actionCostStat = value; }
    public int ActionCost => ActionCostStat.Value;
    protected float actionCostMultiplier = 1f;
    public float ActionCostMultiplier { get => actionCostMultiplier; set => actionCostMultiplier = value; }
    public bool ActionInProgress { get; private set; }
    public Tile CurTile => Stats?.CurTile;
    

    public CharacterStats Stats;
    protected int? totalCost = null;
    public int? TotalCost { get=> totalCost; set => totalCost = value; }

    protected virtual void Awake()
    {
        Stats = GetComponent<CharacterStats>();
    }

    public virtual void Init()
    {
        CurrentTime = TurnManager.Instance.globalTime;
        NextActionTime = CurrentTime + ActionCost;
    }
    public abstract void PerformAction();
    public void SetNextActionCost(int cost)
    {
        TotalCost = cost;
    }
    public virtual int GetModifiedActionCost()
    {
        if (TotalCost.HasValue)
        {
            int cost = TotalCost.Value;
            TotalCost = null;            // 한 번 쓰고 자동 초기화
            return cost;
        }
        return Mathf.Max(1, Mathf.RoundToInt(ActionCost * ActionCostMultiplier));
    }

    public void StartTurn()
    {
        ActionInProgress= true;
        PerformAction();
    }

    public void AdvanceTime(int cost)
    {
        CurrentTime = NextActionTime;
        NextActionTime += cost;
    }

    public void OnTurnEnd()
    {
        if (ActionInProgress)
            ActionInProgress = false;
    }

    public bool IsPlayer => this is PlayerUnit;


    
}
