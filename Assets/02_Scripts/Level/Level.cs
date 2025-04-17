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
    public TileDataBase tileDataBase;
    public BiomeSet biomeSet;
    public Tile startTile;
    public Tile endTile;
    public Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
    public bool isPainted = false;

}