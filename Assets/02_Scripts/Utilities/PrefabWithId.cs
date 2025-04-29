using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabWithId : ScriptableObject
{
    [Header("Id & Prefab")]
    public int id;
    public GameObject prefab;
}
