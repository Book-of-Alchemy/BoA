using System.Collections;
using System.Collections.Generic;
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
    public List<RoomPreset> nomalRooms;

    public List<Sprite> groundTiles;
    public List<GameObject> mapObjectList;


    [Header("AutoWall")]
    public List<AutoWallTileSet> wallAutoTileSet;

    private void OnValidate()
    {
        
    }
}
