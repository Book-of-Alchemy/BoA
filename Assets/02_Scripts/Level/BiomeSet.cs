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
    public BiomeType biomeType;
    public List<Sprite> groundTiles;
    public List<GameObject> mapObjectList;

    [Header("AutoWall")]
    public List<AutoWallTileSet> wallAutoTileSet;
}
