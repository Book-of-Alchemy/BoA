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
    public EnvironmentType environmentType;
    public bool isDoorPoint;

    public int MoveCost => CalculateMoveCost();
    public int AstarCost => CalculateAstarCost();
    public bool IsWalkable => CalculateIsWalkable();
    public bool isOccupied;

    public bool isExplored;//향후 옵저버패턴 적용 이는 화면 표시방식에 적용 될예정이며 실제 entity의 시야와는 별개로 이용
    public bool isOnSight;
    public CharacterStats characterStats;
    //위에 올라간 entity
    //위에 올라간 mapObject 인스턴스

    private int CalculateMoveCost()
    {
        int cost = environmentType switch
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
        int cost = environmentType switch
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
