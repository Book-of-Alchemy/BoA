

using System;
using System.Collections.Generic;

public class StayIdleBehaviour : IdleBaseBehaviour
{
    public override void Action()
    {
        if (CurTile.groundEffect != null || CurTile.airEffect != null)
        {
            List<Tile> tiles = TileUtility.GetAdjacentTileList(CurTile.curLevel, CurTile, true);
            tiles.RemoveAll(t => t.tileType != TileType.ground || t.CharacterStatsOnTile != null);
            Tile target = tiles[UnityEngine.Random.Range(0, tiles.Count)];
            controller.MoveTo(
            target.gridPosition,
            () =>
            {
            CurTile.CharacterStatsOnTile = null;
             CurTile = target;
            CurTile.CharacterStatsOnTile = enemyStats;
            EndTurn();
            }
            );
            return;
        }
        EndTurn();
    }
}
