using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileUtility 
{
    public static Tile GetAdjacentTile(Level level,Tile tile, FourDir dir)
    {
        Vector2Int offset = dir switch
        {
            FourDir.up => Vector2Int.up,
            FourDir.down => Vector2Int.down,
            FourDir.left => Vector2Int.left,
            FourDir.right => Vector2Int.right,
            _ => Vector2Int.zero
        };

        Vector2Int targetPos = tile.gridPosition + offset;

        if (level.tiles.TryGetValue(targetPos, out Tile neighborTile))
        {
            return neighborTile;
        }

        return null;
    }

    public static Tile GetAdjacentTile(Level level,Tile tile, EightDir dir)
    {
        Vector2Int offset = dir switch
        {
            EightDir.upperLeft => new Vector2Int(-1, 1),
            EightDir.upperCenter => new Vector2Int(0, 1),
            EightDir.upperRight => new Vector2Int(1, 1),
            EightDir.centerLeft => new Vector2Int(-1, 0),
            EightDir.centerRight => new Vector2Int(1, 0),
            EightDir.lowerLeft => new Vector2Int(-1, -1),
            EightDir.lowerCenter => new Vector2Int(0, -1),
            EightDir.lowerRight => new Vector2Int(1, -1),
            _ => Vector2Int.zero
        };

        Vector2Int targetPos = tile.gridPosition + offset;

        if (level.tiles.TryGetValue(targetPos, out Tile neighborTile))
        {
            return neighborTile;
        }

        return null;
    }

    public static List<Tile> GetAdjacentTileList(Level level,Tile tile, bool isEightDir = false)
    {
        if (!isEightDir)
        {
            List<Tile> checkList = new List<Tile>();
            for (int i = 0; i < Enum.GetValues(typeof(FourDir)).Length; i++)
            {
                checkList.Add(GetAdjacentTile(level,tile, (FourDir)i));
            }
            return checkList;
        }
        else
        {
            List<Tile> checkList = new List<Tile>();
            for (int i = 0; i < Enum.GetValues(typeof(EightDir)).Length; i++)
            {
                checkList.Add(GetAdjacentTile(level,tile, (EightDir)i));
            }
            return checkList;
        }
    }

    public static List<Tile> GetVisibleTiles(Level level,Tile origin, int viewRange)
    {
        List<Tile> visibleTiles = new List<Tile>();
        Queue<Tile> frontier = new Queue<Tile>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        frontier.Enqueue(origin);
        visited.Add(origin.gridPosition);
        visibleTiles.Add(origin);

        while (frontier.Count > 0)
        {
            Tile current = frontier.Dequeue();
            int currentDistance = Mathf.Abs(current.gridPosition.x - origin.gridPosition.x) +
                                  Mathf.Abs(current.gridPosition.y - origin.gridPosition.y);

            if (currentDistance >= viewRange)
                continue;

            foreach (Tile neighbor in GetAdjacentTileList(level,current, true))
            {
                if (neighbor == null) continue;

                Vector2Int pos = neighbor.gridPosition;
                if (visited.Contains(pos)) continue;

                visited.Add(pos);
                visibleTiles.Add(neighbor);


                if (neighbor.canSeeThrough)
                {
                    frontier.Enqueue(neighbor);
                }
            }
        }

        return visibleTiles;
    }
}
