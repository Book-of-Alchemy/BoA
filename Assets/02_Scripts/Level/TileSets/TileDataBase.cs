using System.Collections;
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

    private void OnValidate()
    {
        biomsetByID.Clear();

        foreach(var biome in biomeSet)
        {
            if (biomsetByID.ContainsKey(biome.id))
                Debug.LogWarning($"중복된 바이옴입니다 ID : {biome.id}");

            biomsetByID[biome.id] = biome;
        }
    }
}
