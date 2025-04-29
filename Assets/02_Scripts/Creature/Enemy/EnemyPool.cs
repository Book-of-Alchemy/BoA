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
       
    }

    void SetEnemyStat(int id, int level, EnemyStats enemy)
    {
        if (!dataById.TryGetValue(id, out var data))
            return;


        enemy.level = level;

        // 체력 설정추가
        // 공방
        enemy.attackMin = data.base_atk + data.atk_per_level * (level - 1);
        enemy.attackMax = enemy.attackMin + 5f;
        enemy.defense = data.base_def + data.def_per_level * (level - 1);
        // 속성 저항
        enemy.fire = data.resist_fire;
        enemy.water = data.resist_water;
        enemy.ice = data.resist_cold;
        enemy.electric = data.resist_wind;
        enemy.earth = data.resist_lightning;
        enemy.wind = data.resist_earth;
        enemy.light = data.resist_light;
        enemy.dark = data.resist_dark;
        // 시야 사거리
        enemy.attackRange = data.attack_range;
        enemy.visionRange = data.sight;
    }
}
