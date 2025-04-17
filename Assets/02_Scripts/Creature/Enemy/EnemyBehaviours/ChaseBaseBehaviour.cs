using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBaseBehaviour : BaseBehaviour
{
    public override void Excute()
    {
        Level level = enemyStats.curLevel;
        Tile tile = enemyStats.curTile;
        int attackRange = enemyStats.attackRange;
        List<Tile> attackRangeTile = (attackRange == 1 ?
            TileUtility.GetAdjacentTileList(level, tile, true) :
            TileUtility.GetRangedTile(level, tile, attackRange));


    }
}
