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
    public abstract int ActionCost { get; }
    protected EnemyController controller;
    protected EnemyStats enemyStats;
    protected Level level => enemyStats.curLevel;
    protected Tile CurTile
    {
        get => enemyStats.CurTile;
        set => enemyStats.CurTile = value;
    }
    protected int attackRange => enemyStats.attackRange;
    protected List<Tile> attackRangeTile => (attackRange == 1 ?
            TileUtility.GetAdjacentTileList(level, CurTile, true) :
            TileUtility.GetRangedTile(level, CurTile, attackRange));

    protected List<Tile> vision => enemyStats.tilesOnVision;


    protected virtual void Awake()
    {
        controller = GetComponent<EnemyController>();
        enemyStats = GetComponent<EnemyStats>();
    }

    public abstract void Excute();

    public abstract bool StateCheck();
    public abstract void Action();
}
