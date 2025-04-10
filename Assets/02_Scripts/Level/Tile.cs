using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum TileType
{
    empty = 0,
    ground,
    wall,
    door,
    secretWall,
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
    public bool isDoorPoint;

    public int MoveCost => CalculateMoveCost();
    public int AstarCost => CalculateAstarCost();
    public bool isWalkable => CalculateIsWalkable();
    public bool isOccupied;

    public bool isExplored;//향후 옵저버패턴 적용
    public bool isOnSight;
    //위에 올라간 entity
    //위에 올라간 mapObject 인스턴스

    private int CalculateMoveCost()
    {
        int cost = environment switch
        {
            EnvironmentType.swamp => 2,
            EnvironmentType.lava => 2,
            EnvironmentType.grease => 1,
            EnvironmentType.water => 1,
            _ => 1
        };

        return cost;
    }

    private int CalculateAstarCost()
    {
        int cost = environment switch
        {
            EnvironmentType.swamp => 6,
            EnvironmentType.lava => 6,
            EnvironmentType.grease => 2,
            EnvironmentType.water => 1,
            _ => 1
        };

        return cost;
    }

    private bool CalculateIsWalkable()
    {
        bool isWalkable = tileType switch
        {
            TileType.wall => false,
            TileType.secretWall => false,
            TileType.ground => !isOccupied,
            TileType.door => !isOccupied,
            TileType.empty => false,
            _ => false
        };

        return isWalkable;
    }
}
