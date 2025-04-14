using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

public enum BiomeType
{
    Forest = 0,
}


[CreateAssetMenu(menuName = "Map/Biome Set")]
public class BiomeSet : ScriptableObject
{
    public int id;
    public string name_kr;
    public BiomeType biomeType;
    public List<RoomPreset> roomPresets;
    [HideInInspector]
    public Dictionary<RoomType, List<RoomPreset>> roomsByType = new();

    [Header("TileSets")]
    public List<GroundTileSet> groundTileSet;
    public List<AutoWallTileSet> wallAutoTileSet;
    public List<AutoEnvironmentalSet> environmentalTileSet;
    public List<GameObject> mapObjectList;


   

    private void OnValidate()
    {
        CategorizeRoomPresets();
    }

    void CategorizeRoomPresets()
    {
        roomsByType = new();

        foreach (RoomPreset preset in roomPresets)
        {
            if (!roomsByType.ContainsKey(preset.roomType))
            {
                roomsByType[preset.roomType] = new List<RoomPreset>();
            }

            roomsByType[preset.roomType].Add(preset);
        }
    }

    public List<RoomPreset> GetPresets(RoomType type)
    {
        if (roomsByType.TryGetValue(type, out var presets))
        {
            return presets;
        }

        return new List<RoomPreset>(); // 없는 경우 빈 리스트 반환
    }
}
