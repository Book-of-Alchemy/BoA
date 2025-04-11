using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    normal,
    treasure,
    trap,
    secret,
    boss
}

[System.Serializable]
public class TileInfoForRoom
{
    public Vector2Int position;
    public TileType tileType;
    public EnvironmentType environmentType;
    public bool isDoorPoint;
    //맵 오브젝트
    //아이템

    public TileInfoForRoom(Vector2Int position, TileType tileType = TileType.ground, EnvironmentType environmentType = EnvironmentType.none, bool isDoorPoint = false)
    {
        this.position = position;
        this.tileType = tileType;
        this.environmentType = environmentType;
        this.isDoorPoint = isDoorPoint;
    }
}

[CreateAssetMenu(fileName = "RoomPreset", menuName = "Map/Room Preset")]
public class RoomPreset : ScriptableObject
{
    public int id;
    public string roomName;
    public int biome_id;
    public RoomType roomType;
    public Vector2Int roomSize;


    public Dictionary<Vector2Int, TileInfoForRoom> tileInfo;

    //[HideInInspector]
    [SerializeField]
    private List<TileInfoForRoom> tileList;

    public void RebuildTileListFromDictionary()
    {
        if (tileInfo != null)
            tileList = new List<TileInfoForRoom>(tileInfo.Values);
    }

}

