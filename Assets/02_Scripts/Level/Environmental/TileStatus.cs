using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileStatusEffect : MonoBehaviour, ITurnProcessor
{
    protected int currentTime = 0;
    public int CurrentTime { get => currentTime; set => currentTime = value; }
    protected int nextActionTime = 0;
    public int NextActionTime { get => nextActionTime; set => nextActionTime = value; }
    public int ActionCost => 10;
    public bool ActionInProgress { get; private set; }
    protected Tile curTile;
    public Tile CurTile { get => curTile; set => curTile = value; }

    protected virtual void Awake()
    {
    }

    public virtual void Init()
    {
        CurrentTime = TurnManager.Instance.globalTime;
        NextActionTime = CurrentTime + ActionCost;
    }
    public abstract void PerformAction();

    public void StartTurn()
    {
        ActionInProgress = true;
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
}

