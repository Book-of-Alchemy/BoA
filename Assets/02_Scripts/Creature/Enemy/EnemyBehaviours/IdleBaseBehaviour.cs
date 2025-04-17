using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBaseBehaviour : BaseBehaviour
{
    public override void Excute()
    {
        List<Tile> vision = enemyStats.TilesOnVision;

        foreach (Tile tile in vision)
        {
            if (tile.characterStats is PlayerStats player)
            {
                controller.ChangeState(EnemyState.Chase);
                controller.lastCheckedTile = tile;
                break;
            }
        }
    }
}
