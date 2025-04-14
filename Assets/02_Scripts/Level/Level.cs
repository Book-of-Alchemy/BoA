using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FourDir
{
    up,
    right,
    down,
    left,    
}

public enum EightDir
{
    upperLeft,
    upperCenter,
    upperRight,
    centerLeft,
    centerRight,
    lowerLeft,
    lowerCenter,
    lowerRight,
}

public class Level : MonoBehaviour
{
    public BiomeSet biomeSet;
    public Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();


    public Tile GetAdjacentTile(Tile tile, FourDir dir)
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

        if (tiles.TryGetValue(targetPos, out Tile neighborTile))
        {
            return neighborTile;
        }

        return null; 
    }

    public Tile GetAdjacentTile(Tile tile, EightDir dir)
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

        if (tiles.TryGetValue(targetPos, out Tile neighborTile))
        {
            return neighborTile;
        }

        return null;
    }

    public List<Tile> GetAdjacentTileList(Tile tile ,bool isEightDir = false)
    {
        if (!isEightDir)
        {
            List<Tile> checkList = new List<Tile>();
            for (int i = 0; i < Enum.GetValues(typeof(FourDir)).Length; i++)
            {
                checkList.Add(GetAdjacentTile(tile, (FourDir)i));
            }
            return checkList;
        }
        else
        {
            List<Tile> checkList = new List<Tile>();
            for (int i = 0; i < Enum.GetValues(typeof(EightDir)).Length; i++)
            {
                checkList.Add(GetAdjacentTile(tile, (EightDir)i));
            }
            return checkList;
        }
    }


}