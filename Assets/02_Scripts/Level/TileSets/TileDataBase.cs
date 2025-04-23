using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/TileDataBase")]
public class TileDataBase : ScriptableObject

{
    public List<BiomeSet> biomeSet;
    public List<AutoEnvironmentalSet> environmentalTileSet;
    public List<TrapData> trapData;
    public List<MapObjectData> mapObjectData;
    public GameObject ladder;
}
