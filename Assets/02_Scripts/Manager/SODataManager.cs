using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SODataManager : Singleton<SODataManager>
{
    public TileDataBase tileDataBase;
    public EnemyDataBase enemyDataBase;
    public QuestDataBase questDataBase;
    public GameObject playerPrefab;
}
