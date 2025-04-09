using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    empty = 0,
    ground,
    wall
}

public enum EnvironmentType
{
    none = 0,
    swamp,
    lava,
    grease,
    water,
}

[System.Serializable]
public class Tile
{
    public Vector2Int gridPosition;
    public TileType tileType;
    public EnvironmentType environment;

    public bool isWalkable;
    public bool isOccupied;

    public bool isExplored;//향후 옵저버패턴 적용
    public bool isOnSight;
    //위에 올라간 entity
    //위에 올라간 mapObject 정보
}
