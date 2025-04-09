using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    normal,
    treasure,
    trap,
    boss
}

[System.Serializable]
public class TileInfoForRoom
{
    public Vector2Int position;
    public TileType tileType;
    public EnvironmentType environmentType;
    //맵 오브젝트
    //아이템

    public TileInfoForRoom(Vector2Int position, TileType tileType = TileType.ground, EnvironmentType environmentType = EnvironmentType.none)
    {
        this.position = position;
        this.tileType = tileType;
        this.environmentType = environmentType;
    }
}

[CreateAssetMenu(fileName = "RoomPreset", menuName = "Map/Room Preset")]
public class RoomPreset : ScriptableObject
{
    public string roomName;
    public Vector2Int roomSize;
    public RoomType roomType;

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

