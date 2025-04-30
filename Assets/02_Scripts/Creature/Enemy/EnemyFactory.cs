using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : Singleton<EnemyFactory>
{
    public EnemyPool enemyPool;
    public Dictionary<int, EnemyData> enemyDataById;
    public Dictionary<int, List<EnemyData>> enemyDataByBiome;
    public float spawnNoEnemyChance = 0.2f;
    public float spawnOneEnemyChance = 0.3f;
    public float spawnTwoEnemyChance = 0.3f;
    public float spawnThreeEnemyChance = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        enemyDataById = SODataManager.Instance.enemyDataBase.enemyDataById;
        enemyDataByBiome = SODataManager.Instance.enemyDataBase.enemyDataByBiome;
    }
    public void EnemySpawnAtStart(Level level)
    {
        List<Leaf> leavesWithoutStart = new List<Leaf>(level.seletedLeaves);
        leavesWithoutStart.Remove(level.startLeaf);
        List<EnemyData> enemyData = GetProperEnemies(level);

        foreach (var leaf in leavesWithoutStart)
        {
            int spawnCount = GetRandomEnemyQuntity();
            if (spawnCount == 0) continue;

            List<Tile> availableTiles = new List<Tile>();

            foreach (var pos in TileUtility.GetPositionsInRect(leaf.rect))
            {
                if (level.tiles.TryGetValue(pos, out Tile tile))
                {
                    if (tile.tileType == TileType.ground && !tile.isOccupied)
                    {
                        availableTiles.Add(tile);
                    }
                }
            }

            for (int i = 0; i < spawnCount && availableTiles.Count > 0; i++)
            {
                Tile targetTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
                availableTiles.Remove(targetTile);

                int id = GetRandomEnemyId(enemyData);
                EnemyStats enemy = enemyPool.GetFromPool(id, targetTile.gridPosition, level.transform);
                
                SetEnemyStat(GetCurEnemyLevel(level), enemy, enemyDataById[id]);
                InitEnemy(enemy, targetTile);
            }
        }
    }

    int GetRandomEnemyQuntity()
    {
        float rand = Random.value;
        float spawnChance = spawnNoEnemyChance;
        if (rand < spawnChance)
            return 0;
        spawnChance += spawnOneEnemyChance;
        if (rand < spawnChance)
            return 1;
        spawnChance += spawnTwoEnemyChance;
        if (rand < spawnChance)
            return 2;
        spawnChance += spawnThreeEnemyChance;
        if (rand < spawnChance)
            return 3;

        return 0;
    }

    List<EnemyData> GetProperEnemies(Level level)
    {
        if (!enemyDataByBiome.ContainsKey(level.biomeSet.id))
            return null;

        List<EnemyData> enemyDatas = new List<EnemyData>(enemyDataByBiome[level.biomeSet.id]);
        int curEnemyLevel = GetCurEnemyLevel(level);
        enemyDatas.RemoveAll(enemy => curEnemyLevel > enemy.max_level);
        enemyDatas.RemoveAll(enemy => curEnemyLevel < enemy.min_level);

        return enemyDatas;
    }

    int GetCurEnemyLevel(Level level)
    { 
        return level.questData.base_monster_level + level.questData.level_per_floor * level.curDepth;
    }

    int GetRandomEnemyId(List<EnemyData> enemies)
    {
        return enemies[UnityEngine.Random.Range(0, enemies.Count - 1)].id;
    }

    void SetEnemyStat(int curEnemyLevel, EnemyStats enemy, EnemyData enemyData)
    {
        //if (!enemyDataById.TryGetValue(id, out var data))
            //return;


        enemy.level = curEnemyLevel;

        // 체력 설정추가
        enemy.MaxHealth = enemyData.base_hp + enemyData.hp_per_level * curEnemyLevel;
        enemy.CurrentHealth = enemyData.base_hp + enemyData.hp_per_level * curEnemyLevel;
        // 공방
        enemy.attackMin = enemyData.base_atk + enemyData.atk_per_level * curEnemyLevel;
        enemy.attackMax = enemy.attackMin + 5f;
        enemy.defense = enemyData.base_def + enemyData.def_per_level * curEnemyLevel;
        // 속성 저항
        enemy.fire = enemyData.resist_fire;
        enemy.water = enemyData.resist_water;
        enemy.ice = enemyData.resist_cold;
        enemy.electric = enemyData.resist_wind;
        enemy.earth = enemyData.resist_lightning;
        enemy.wind = enemyData.resist_earth;
        enemy.light = enemyData.resist_light;
        enemy.dark = enemyData.resist_dark;
        // 시야 사거리
        enemy.attackRange = enemyData.attack_range;
        enemy.visionRange = enemyData.sight;
    }

    void InitEnemy(EnemyStats enemy, Tile tile)
    {
        enemy.CurTile = tile;
        enemy.curLevel = tile.curLevel;
        tile.CharacterStatsOnTile = enemy;
        //tile.isOccupied = true;
        TurnManager.Instance.AddUnit(enemy.unitBase);
    }


}
