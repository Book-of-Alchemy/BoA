using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyDataBase")]
public class EnemyDataBase : ScriptableObject
{
    public List<EnemyData> enemyData;
    public Dictionary<int, EnemyData> enemyDataById = new Dictionary<int, EnemyData>();
    public Dictionary<int, List<EnemyData>> enemyDataByBiome = new Dictionary<int, List<EnemyData>>();

    private void OnEnable()
    {
        enemyDataById.Clear();
        enemyDataByBiome.Clear();

        foreach (var enemy in enemyData)
        {
            if (enemy == null) continue;

            if (!enemyDataById.ContainsKey(enemy.id))
            {
                enemyDataById.Add(enemy.id, enemy);
            }
            else
            {
            }

            if (!enemyDataByBiome.ContainsKey(enemy.biome_id))
            {
                enemyDataByBiome[enemy.biome_id] = new List<EnemyData>();
            }

            enemyDataByBiome[enemy.biome_id].Add(enemy);
        }
    }

    public EnemyData GetEnemyById(int id)
    {
        enemyDataById.TryGetValue(id, out var data);
        return data;
    }

    public List<EnemyData> GetEnemiesByBiome(int biomeId)
    {
        enemyDataByBiome.TryGetValue(biomeId, out var list);
        return list ?? new List<EnemyData>();
    }
}
