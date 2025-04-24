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

public enum RoomSizeType
{
    small,
    medium,
    large,
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
/// <summary>
/// 타일 배치는 항상 roomsize에서 상하좌우로 한칸씩 이격하여 만들어야함!!
/// </summary>
[CreateAssetMenu(fileName = "RoomPreset", menuName = "Map/Room Preset")]
public class RoomPreset : ScriptableObject
{
    public int id;
    public string roomName;
    public int biome_id;
    public RoomType roomType;
    public RoomSizeType roomSizeType;
    public Vector2Int roomSize;


    public Dictionary<Vector2Int, TileInfoForRoom> tileInfo;

    //[HideInInspector]
    [SerializeField]
    private List<TileInfoForRoom> tileList;

    private void OnValidate()
    {
        RebuildDictionaryFromTileList();
        roomSizeType = GetRoomSizeType();
    }

    public void RebuildTileListFromDictionary()
    {
        if (tileInfo != null)
            tileList = new List<TileInfoForRoom>(tileInfo.Values);
    }

    void RebuildDictionaryFromTileList()
    {
        if (tileInfo == null)
            tileInfo = new Dictionary<Vector2Int, TileInfoForRoom>();

        tileInfo.Clear(); // 중복 방지 (선택사항)

        foreach (var tile in tileList)
        {
            tileInfo[tile.position] = tile;
        }
    }

    RoomSizeType GetRoomSizeType()
    {
        int length = roomSize.x > roomSize.y ? roomSize.x : roomSize.y;
        RoomSizeType size = (length) switch
        {

            < 10 => RoomSizeType.small,
            < 16 => RoomSizeType.medium,
            _ => RoomSizeType.large,
        };

        return size;
    }
}

