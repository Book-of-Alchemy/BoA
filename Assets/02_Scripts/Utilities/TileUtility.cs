using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public static class TileUtility 
{
    /// <summary>
    /// 인접한 타일에 특정 방향의 타일을 반환함 enum fourdir 사용시 4방향에서 받아오기 가능하며 eightdir 사용시 8방향에서 받아오기가능함
    /// </summary>
    /// <param name="level"></param>
    /// <param name="tile"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Tile GetAdjacentTile(Level level,Tile tile, FourDir dir)
    {
        if (level == null || tile == null) return null;
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
    /// <summary>
    /// 위 메서드의 오버로드 버전
    /// </summary>
    /// <param name="level"></param>
    /// <param name="tile"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Tile GetAdjacentTile(Level level,Tile tile, EightDir dir)
    {
        if (level == null || tile == null) return null;
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
    /// <summary>
    /// 인접한 타일리스트를 반환한다 isEightDir true시 8방향 false시 4방향만 받아옴
    /// </summary>
    /// <param name="level"></param>
    /// <param name="tile"></param>
    /// <param name="isEightDir"></param>
    /// <returns></returns>
    public static List<Tile> GetAdjacentTileList(Level level,Tile tile, bool isEightDir = false)
    {
        if (level == null || tile == null) return null;
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
    /// <summary>
    /// range반경의 타일리스트를 반환한다 indludeself true 시 자기자신까지 반환 ex_ 아이템 사용 범위 리스트 받을시 제외
    /// </summary>
    /// <param name="level"></param>
    /// <param name="tile"></param>
    /// <param name="range"></param>
    /// <param name="isIncludeSelf"></param>
    /// <returns></returns>
    public static List<Tile> GetRangedTile(Level level, Tile tile, int range, bool isIncludeSelf = false)
    {
        if (level == null || tile == null) return null;
        List<Tile> rangedTiles = new List<Tile>();

        foreach (var kvp in level.tiles)
        {
            Tile target = kvp.Value;
            int dist = Mathf.Abs(target.gridPosition.x - tile.gridPosition.x) +
                       Mathf.Abs(target.gridPosition.y - tile.gridPosition.y);

            if (dist <= range)
            {
                rangedTiles.Add(target);
            }
        }

        if (!isIncludeSelf)
            rangedTiles.Remove(tile);

        return rangedTiles;
    }

    /// <summary>
    /// veiwRange 만큼의 범위에서 눈에 보이는 타일리스트를 가져온다
    /// 해당 위치까지의 1자로 된 tile 리스트를 생성후 tile seeThrough false인 tile까지만 추가함
    /// </summary>
    /// <param name="level"></param>
    /// <param name="tile"></param>
    /// <param name="viewRange"></param>
    /// <returns></returns>
    public static List<Tile> GetVisibleTiles(Level level, Tile tile, int viewRange)
    {
        if (level == null || tile == null) return null;
        List<Tile> visibleTiles = new List<Tile>();

        foreach (var tilePair in level.tiles)
        {
            Tile target = tilePair.Value;
            int dist = Mathf.Abs(target.gridPosition.x - tile.gridPosition.x) +
                       Mathf.Abs(target.gridPosition.y - tile.gridPosition.y);

            if (dist <= viewRange && IsTileVisible(level, tile, target))
            {
                visibleTiles.Add(target);
            }
        }

        return visibleTiles;
    }

    /// <summary>
    /// 1자로 선을 그은뒤 해당 타일까지 도달하는데 seethrough false인 타일이 있는지 검사함
    /// </summary>
    /// <param name="level"></param>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsTileVisible(Level level, Tile origin, Tile target)
    {
        List<Vector2Int> line = GetLine(origin.gridPosition, target.gridPosition);

        foreach (Vector2Int pos in line)
        {
            if (pos == target.gridPosition) break; // 마지막 타일은 검사하지 않음

            if (level.tiles.TryGetValue(pos, out Tile tile))
            {
                if (!tile.canSeeThrough)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 타일상에 레스터로된 1자선을 그어줌
    /// </summary>
    /// <param name="level"></param>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="isIncludeSelf"></param>
    /// <returns></returns>
    public static List<Tile> GetLineTile(Level level, Tile origin, Tile target, bool isIncludeSelf = false)
    {
        if (level == null || origin == null) return null;
        List<Tile> tiles = new List<Tile>();
        List<Vector2Int> line = GetLine(origin.gridPosition, target.gridPosition);

        foreach (Vector2Int pos in line)
        {
            if(level.tiles.TryGetValue(pos, out Tile tile))
                tiles.Add(tile);
        }

        if (!isIncludeSelf)
            tiles.Remove(origin);

        return tiles;
    }

    public static List<Vector2Int> GetLine(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> line = new List<Vector2Int>();

        int x0 = start.x;
        int y0 = start.y;
        int x1 = end.x;
        int y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            line.Add(new Vector2Int(x0, y0));

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return line;
    }
}
