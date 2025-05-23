using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/TileDataBase")]
public class TileDataBase : ScriptableObject

{
    public List<BiomeSet> biomeSet;
    public List<TrapData> trapData;
    public List<MapObjectData> mapObjectData;
    public GameObject ladder;

    public Dictionary<int, BiomeSet> biomsetByID = new Dictionary<int, BiomeSet>();

    private void OnEnable()
    {
        Arrange();
    }

    public void Arrange()
    {
        biomsetByID.Clear();

        foreach (var biome in biomeSet)
        {


            biomsetByID[biome.id] = biome;
        }
    }
}
