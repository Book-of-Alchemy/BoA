using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManger : Singleton<TileManger>
{
    public Level curLevel;
    public LevelGenerator levelGenerator;
    public GameObject groundPrefab;
    public GameObject wallPrefab;


}
