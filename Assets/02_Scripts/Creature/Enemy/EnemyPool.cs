using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<EnemyStats>
{
    Dictionary<int, GameObject> enemyById = new Dictionary<int, GameObject>();
    Dictionary<int, EnemyData> enemyDataById;
    protected override void Awake()
    {
        Init();
        base.Awake();
    }

    void Init()
    {
        List<EnemyData> data = SODataManager.Instance.enemyDataBase.enemyData;
        enemyDataById = SODataManager.Instance.enemyDataBase.enemyDataById;
        foreach (var enemy in data)
        {
            prefabs.Add(enemy.prefab);
            enemyById[enemy.id] = enemy.prefab;
        }
    }

    public EnemyStats GetFromPool(int id, Transform spawnPosition, Transform newParent = null)
    {
        GameObject prefab = enemyById[id];
        EnemyStats obj;
        if (poolDictionary.ContainsKey(prefab.name) && poolDictionary[prefab.name].Count > 0)
        {
            obj = poolDictionary[prefab.name].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab).GetComponent<EnemyStats>();
            obj.name = prefab.name;
        }
        //향후 setEnemystat 추가
        if (newParent != null) obj.transform.SetParent(newParent);
        obj.transform.position = spawnPosition.position;
        obj.gameObject.SetActive(true);

        return obj;
    }

    void SetEnemyStat(int id, int level, EnemyStats enemy)
    {
        if (!enemyDataById.TryGetValue(id, out var data))
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
