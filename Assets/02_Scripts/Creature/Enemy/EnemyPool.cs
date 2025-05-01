using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectByIdPool<EnemyStats, EnemyData>
{

    protected override void Awake()
    {
        Init();
        base.Awake();
    }

    void Init()
    {
        prefabWithIds = SODataManager.Instance.enemyDataBase.enemyData;
        EnemyFactory.Instance.enemyPool = this;
    }

    
}
