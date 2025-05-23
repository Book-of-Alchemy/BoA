using System;
using System.Collections.Generic;
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
    Shallow_Water,
    Lava,
    Oil,
    Mud,
    Toxic_Air,
    Electrofied_Water,
    Flame,
    Solidfied_Lava,
    Fog,
    Iced_Water,
    Slimed_Field,
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

    public int AstarCost => CalculateAstarCost();
    public bool IsWalkable => CalculateIsWalkable();
    public bool IsOccupied =  false;// map object or characterstat여부에 따라 자동으로 결정
    public bool CanSeeThrough => CalculateCanSeeThrough();//벽타일 or map object의 canseethrough 여부에 따라 자동으로 결정
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
    public TileEffect groundEffect;
    public TileEffect airEffect;
    public void RefreshSight()
    {
        onIsOnSightChanged?.Invoke();
    }


    private int CalculateAstarCost()
    {
        int cost = environmentType switch
        {
            EnvironmentType.Lava => 6,
            EnvironmentType.Oil => 6,
            EnvironmentType.Mud => 6,
            EnvironmentType.Toxic_Air => 6,
            EnvironmentType.Electrofied_Water => 6,
            EnvironmentType.Flame => 6,
            EnvironmentType.Solidfied_Lava => 6,
            EnvironmentType.Fog => 6,
            EnvironmentType.Iced_Water => 6,
            EnvironmentType.Slimed_Field => 6,
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
            TileType.ground => !IsOccupied,
            TileType.door => !IsOccupied,
            TileType.empty => false,
            _ => false
        };

        if (characterStatsOnTile != null)
            isWalkable = false;

        return isWalkable;
    }

    private bool CalculateCanSeeThrough()
    {
        bool isCanSeeThrough;
        isCanSeeThrough = tileType switch
        {
            TileType.wall => false,
            TileType.secretWall => false,
            TileType.ground => !IsOccupied,
            TileType.door => !IsOccupied,
            _ => true,
        };
        if (airEffect != null && (airEffect is FogTile || airEffect is ToxicAirTile))
        {
            isCanSeeThrough = false;
        }

        return isCanSeeThrough;
    }

    private bool CalculateIsOccupied()
    {
        if (tileType == TileType.wall)
            return true;

        if (mapObject != null && mapObject.IsOccuPying)
            return true;

        if (characterStatsOnTile != null)
            return true;

        return false;
    }

    public void AffectOnTile(TileReactionResult reactionResult, bool isAir)
    {
        var env = isAir ? airEffect : groundEffect;
        if (env == null) return;

        if (env.EnvType != reactionResult.sourceTileType) return;

        EnvironmentalFactory.Instance.ReturnTileEffect(env);

        if (reactionResult.effect_ID != -1)
            EffectProjectileManager.Instance.PlayEffect(gridPosition, reactionResult.effect_ID);

        if (characterStatsOnTile != null && reactionResult.damage > 0)
        {
            DamageInfo damage = new DamageInfo(reactionResult.damage, reactionResult.damageType, null, characterStatsOnTile);
            characterStatsOnTile.TakeDamage(damage);
        }

        EnvironmentalFactory.Instance.GetEnvironment(reactionResult.resultTileType, this, curLevel);
    }

}
