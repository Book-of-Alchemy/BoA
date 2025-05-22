using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreSummonChaseBehaviour : ChaseBaseBehaviour, ICooltime
{
    public int lefttime => Mathf.Max(0, availableTime - TurnManager.Instance.globalTime); // 남은 턴
    public int coolTime { get; set; } // 쿨타임 시간
    public int availableTime { get; set; }

    protected override void Awake()
    {
        base.Awake();
        coolTime = 150;
        availableTime = 0;
    }
    public override void Action()
    {
        int currentTime = TurnManager.Instance.globalTime;

        if (currentTime >= availableTime)
        {
            SummonGoblinArchers();
            availableTime = TurnManager.Instance.globalTime + coolTime;
            controller.curSkill = null;
            controller.Skill_1();
            EndTurn();
            return;
        }

        List<Tile> path = AStarPathfinder.FindPathForCreature(CurTile, controller.LastCheckedTile, level);
        if (path == null)
        {
            EndTurn();
            return;
        }
        if (path.Count > 15 || path.Count <= 1)
        {
            EndTurn();
            return;
        }

        MoveTo(path[1]);
    }

    void SummonGoblinArchers()
    {
        List<Tile> available = TileUtility.GetItemRangedTile(CurTile.curLevel, CurTile, 4);

        Dictionary<string, Tile> farthestTiles = new Dictionary<string, Tile>
        {
            { "North", null },
            { "South", null },
            { "East", null },
            { "West", null }
        };

        foreach (Tile tile in available)
        {
            Vector2Int offset = tile.gridPosition - CurTile.gridPosition;

            if (!tile.IsWalkable || tile.CharacterStatsOnTile != null)
                continue;

            if (offset.y > 0) // North
            {
                if (farthestTiles["North"] == null || offset.y > (farthestTiles["North"].gridPosition - CurTile.gridPosition).y)
                    farthestTiles["North"] = tile;
            }
            else if (offset.y < 0) // South
            {
                if (farthestTiles["South"] == null || offset.y < (farthestTiles["South"].gridPosition - CurTile.gridPosition).y)
                    farthestTiles["South"] = tile;
            }

            if (offset.x > 0) // East
            {
                if (farthestTiles["East"] == null || offset.x > (farthestTiles["East"].gridPosition - CurTile.gridPosition).x)
                    farthestTiles["East"] = tile;
            }
            else if (offset.x < 0) // West
            {
                if (farthestTiles["West"] == null || offset.x < (farthestTiles["West"].gridPosition - CurTile.gridPosition).x)
                    farthestTiles["West"] = tile;
            }
        }

        foreach (var pair in farthestTiles)
        {
            Tile spawnTile = pair.Value;
            if (spawnTile != null)
            {
                EnemyFactory.Instance.SpawnEnemy(230006, spawnTile);
            }
        }
    }
}