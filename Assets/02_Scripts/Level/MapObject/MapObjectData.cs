using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapObject/New MapObject")]
public class MapObjectData : ScriptableObject
{
    public int id;
    public string name_kr;
    public int biome_id;
    public Vector2Int gridSize = new Vector2Int(1,1);
    public bool isForQuest = false;
    public int quest_id;
    public GameObject prefab;
}
