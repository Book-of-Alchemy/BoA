using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviour
{
    public void Excute();
    public bool StateCheck();
    public void Action();
}

public abstract class BaseBehaviour : MonoBehaviour, IBehaviour
{
    public abstract float ActionCostMultiPlier { get; }
    [HideInInspector]
    public EnemyController controller;
    [HideInInspector]
    public EnemyStats enemyStats;
    public Level level => enemyStats.curLevel;
    public Tile CurTile
    {
        get => enemyStats.CurTile;
        set => enemyStats.CurTile = value;
    }
    public int attackRange => enemyStats.attackRange;
    public List<Tile> attackRangeTile => (attackRange == 1 ?
            TileUtility.GetAdjacentTileList(level, CurTile, true) :
            TileUtility.GetRangedTile(level, CurTile, attackRange));
    public List<Tile> adjacentiveTile => TileUtility.GetAdjacentTileList(level, CurTile, true);
    public List<Tile> vision => enemyStats.tilesOnVision;


    protected virtual void Awake()
    {
        controller = GetComponent<EnemyController>();
        enemyStats = GetComponent<EnemyStats>();
    }

    public abstract void Excute();

    public abstract bool StateCheck();
    
    public abstract void Action();

    public virtual void EndTurn()
    {
        if (controller == null) 
            return;
        controller.EndTurn();
    }
}
