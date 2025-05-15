using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnvironmentalFactory : Singleton<EnvironmentalFactory>
{
    public EnvironmentalPool environmentalPool;
    EnvironmentalDataBase environmentalDataBase => SODataManager.Instance.environmentalDataBase;
    Dictionary<EnvironmentType, EnvironmentalData> datasByType => environmentalDataBase.datasByType;
    public void GetEnvironment(EnvironmentType type, Tile tile, Level level)
    {
        EnvironmentPrefab prefab = environmentalPool.GetFromPool(tile, level.transform);
        prefab.Initiallize(datasByType[type], tile);
        System.Type effectType = GetEffectTypeByEnvironment(type);
        if (effectType != null && prefab.GetComponent(effectType) == null)
        {
            var t = prefab.gameObject.AddComponent(effectType);

            if (t is IGround and TileEffect groundEffect)
            {
                if (tile.groundEffect != null)
                    ReturnTileEffect(tile.groundEffect);

                tile.groundEffect = groundEffect;
                if (groundEffect is IWater)
                    prefab.AutoTileUpdate();

                prefab.PlayAnimation();
            }
            else if (t is IAir and TileEffect airEffect)
            {
                if (tile.airEffect != null)
                    ReturnTileEffect(tile.airEffect);

                tile.airEffect = airEffect;
                prefab.PlayAnimation();
            }
        }
    }

    public System.Type GetEffectTypeByEnvironment(EnvironmentType type)
    {
        switch (type)
        {
            case EnvironmentType.Shallow_Water: return typeof(ShallowWaterTile);
            case EnvironmentType.Lava: return typeof(LavaTile);
            case EnvironmentType.Oil: return typeof(OilTile);
            case EnvironmentType.Mud: return typeof(MudTile);
            case EnvironmentType.Toxic_Air: return typeof(ToxicAirTile);
            case EnvironmentType.Electrofied_Water: return typeof(ElectrofiedWaterTile);
            case EnvironmentType.Flame: return typeof(FlameTile);
            case EnvironmentType.Solidfied_Lava: return typeof(SolifiedLavaTile);
            case EnvironmentType.Fog: return typeof(FogTile);
            case EnvironmentType.Iced_Water: return typeof(IcedWaterTile);
            case EnvironmentType.Slimed_Field: return typeof(SlimedFieldTile);
            default: return null;
        }
    }

    public void ReturnTileEffect(TileEffect tileEffect)
    {
        environmentalPool.ReturnToPool(tileEffect.prefab);
    }
}
