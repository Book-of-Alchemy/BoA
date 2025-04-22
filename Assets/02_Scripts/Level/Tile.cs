using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

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
    public Level curLevel;
    public Vector2Int gridPosition;
    public TileType tileType;
    public EnvironmentType environmentType;
    public bool isDoorPoint;

    public int MoveCost => CalculateMoveCost();
    public int AstarCost => CalculateAstarCost();
    public bool IsWalkable => CalculateIsWalkable();
    public bool isOccupied;
    public bool canSeeThrough = true;

    public bool isExplored;//향후 옵저버패턴 적용 이는 화면 표시방식에 적용 될예정이며 실제 entity의 시야와는 별개로 이용
    public bool isOnSight;
    public event Action onCharacterChanged;
    private CharacterStats characterStatsOnTile;
    public CharacterStats CharacterStatsOnTile
    {
        get => characterStatsOnTile;
        set
        {
            characterStatsOnTile = value;
            onCharacterChanged?.Invoke();
        }
    }
    [SerializeField]
    private TrapBase trpaOnTile;
    public TrapBase TrpaOnTile
    {
        get => trpaOnTile;
        set
        {
            trpaOnTile = value;
            if (value != null)
                trpaOnTile.Initialize(this);
        }
    }
    public MapObject mapObject;
    public List<DropItem> itemsOnTile = new List<DropItem>();

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

    public int CalculateAstarCostByTrap(bool isConsideringDetected = true)
    {
        int cost = AstarCost;

        if (TrpaOnTile == null)
            return cost;

        if (isConsideringDetected)
        {
            if (TrpaOnTile.IsDetected)
                cost += 6;
        }
        else
            cost += 6;

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
