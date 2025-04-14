using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TilePainter 
{
    public static GameObject game;
    public static void GenerateTileObject(Dictionary<Vector2Int, Tile> tiles, BiomeSet biomeSet)
    {
        foreach(var tile in tiles)
        {
            switch (tile.Value.tileType)
            {
                case TileType.empty:
                    break;
                case TileType.ground:
                    break;
                case TileType.wall:
                    break;
            }
        }
    }
}
