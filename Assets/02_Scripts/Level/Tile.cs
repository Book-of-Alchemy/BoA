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
    public event Action<DamageInfo> onAffectOnEnvironmet;
    public EnvironmentType environmentType;
    public bool isDoorPoint;

    public int MoveCost => CalculateMoveCost();
    public int AstarCost => CalculateAstarCost();
    public bool IsWalkable => CalculateIsWalkable();
    public bool isOccupied;// map object or characterstat여부에 따라 자동으로 결정
    public bool canSeeThrough = true;//벽타일 or map object의 canseethrough 여부에 따라 자동으로 결정
    public event Action onIsExploredChanged;
    private bool isExplored;
    public bool IsExplored
    {
        get => isExplored;
        set
        {
            isExplored = value;
            onIsExploredChanged?.Invoke();
        }
    }
    public event Action onIsOnSightChanged;
    private bool isOnSight;
    public bool IsOnSight
    {
        get => isOnSight;
        set
        {
            isOnSight = value;
            onIsOnSightChanged?.Invoke();
        }
    }
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
            if (trpaOnTile != null)
                trpaOnTile.Initialize(this);
        }
    }
    public MapObject mapObject;
    public List<BaseItem> itemsOnTile = new List<BaseItem>();
    public TileStatusEffect environ;

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

        if (characterStatsOnTile != null)
            isWalkable = false;

        return isWalkable;
    }
    public void AffectOnTile(DamageInfo damageInfo)
    {

    }
}
