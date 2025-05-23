using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class RoomBuilderTool : EditorWindow
{
    private string roomName = "NewRoom";
    private int id;
    private RoomType roomType = RoomType.normal;
    private int biome_id;
    private Vector2Int roomSize = new Vector2Int(10, 10);
    private GameObject root;

    [MenuItem("Tools/Room Builder")]
    public static void ShowWindow()
    {
        GetWindow<RoomBuilderTool>("Room Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Room Preset 생성", EditorStyles.boldLabel);
        id = EditorGUILayout.IntField("Room id", id);
        roomName = EditorGUILayout.TextField("Room 이름", roomName);
        biome_id = EditorGUILayout.IntField("Boim id", biome_id);
        roomType = (RoomType)EditorGUILayout.EnumPopup("Room 타입", roomType);
        roomSize = EditorGUILayout.Vector2IntField("Room 크기", roomSize);
        root = (GameObject)EditorGUILayout.ObjectField("Tile Root Object", root, typeof(GameObject), true);

        if (GUILayout.Button("RoomPreset 생성"))
        {
            GenerateRoomPreset();
        }
    }

    private void GenerateRoomPreset()
    {
        if (root == null)
        {
            return;
        }

        Dictionary<Vector2Int, TileInfoForRoom> tileInfos = new();
        Transform[] allChildren = root.GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {
            TileEditorMarker tile = child.GetComponent<TileEditorMarker>();
            if (tile == null) continue;

            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.y));


            if (tileInfos.ContainsKey(pos))
            {
                tileInfos[pos].position = pos;
                tileInfos[pos].tileType = tile.tileType;
                tileInfos[pos].isDoorPoint = tile.isDoorPoint;

            }
            else tileInfos.Add(pos, new TileInfoForRoom(pos, tile.tileType, EnvironmentType.none, tile.isDoorPoint));
        }

        foreach (Transform child in allChildren)
        {
            EnvironmentEditorMarker environment = child.GetComponent<EnvironmentEditorMarker>();
            if (environment == null) continue;

            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.y));


            if (tileInfos.ContainsKey(pos))
            {
                tileInfos[pos].position = pos;
                tileInfos[pos].tileType = TileType.ground;
                tileInfos[pos].environmentType = environment.envrionmeantType;

            }
            else tileInfos.Add(pos, new TileInfoForRoom(pos, TileType.ground, environment.envrionmeantType));
        }
        //향후 맵 오브젝트 추가 예정

        RoomPreset preset = ScriptableObject.CreateInstance<RoomPreset>();
        preset.id = id;
        preset.roomName = roomName;
        preset.biome_id = biome_id;
        preset.roomType = roomType;
        preset.roomSize = roomSize;
        preset.tileInfo = tileInfos;
        preset.RebuildTileListFromDictionary();

        string path = "Assets/08_ScriptableObjects/RoomPresets";
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder("Assets/08_ScriptableObjects", "RoomPresets");

        string assetPath = $"{path}/{roomName}_Preset.asset";
        AssetDatabase.CreateAsset(preset, assetPath);
        AssetDatabase.SaveAssets();

    }
}
