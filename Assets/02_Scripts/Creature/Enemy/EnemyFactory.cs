using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public EnemyStats SpawnEnemy(int ID, Tile tile)
    {
        if (tile.CharacterStatsOnTile != null) return null;
        EnemyStats enemy = enemyPool.GetFromPool(ID, tile.gridPosition, tile.curLevel.transform);
        SetEnemyStat(GetCurEnemyLevel(tile.curLevel), enemy, enemyDataById[ID]);
        InitEnemy(enemy, tile);
        return enemy;
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
                    if (tile.tileType == TileType.ground && tile.CharacterStatsOnTile == null)
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
        enemyDatas.RemoveAll(enemy => enemy.isBoss);

        return enemyDatas;
    }

    int GetCurEnemyLevel(Level level)
    {
        return level.questData.base_monster_level + level.questData.level_per_floor * level.curDepth;
    }

    int GetRandomEnemyId(List<EnemyData> enemies)
    {
        if(enemies == null || enemies.Count <= 0) return -1;
        return enemies[UnityEngine.Random.Range(0, enemies.Count - 1)].id;
    }

    void SetEnemyStat(int curEnemyLevel, EnemyStats enemy, EnemyData enemyData)
    {
        //if (!enemyDataById.TryGetValue(id, out var data))
        //return;


        enemy.level = curEnemyLevel;

        enemy.statBlock.ResetStatBlock(new Dictionary<StatType, int>
        {
            {StatType.MaxHealth, enemyData.base_hp + enemyData.hp_per_level * curEnemyLevel },
            {StatType.MaxMana, 50 },
            {StatType.Attack, enemyData.base_atk + enemyData.atk_per_level * curEnemyLevel },
            {StatType.Defence, 5 },
            {StatType.CritChance, 10 },
            {StatType.CritDamage, 150 },
            {StatType.Evasion, 5 },
            {StatType.Accuracy, 100 },
            {StatType.VisionRange, enemyData.sight },
            {StatType.AttackRange, enemyData.attack_range },
            {StatType.FireResist, enemyData.resist_fire },
            {StatType.WaterResist, enemyData.resist_water },
            {StatType.IceResist, enemyData.resist_cold },
            {StatType.LightningResist, enemyData.resist_lightning },
            {StatType.EarthResist, enemyData.resist_earth},
            {StatType.WindResist, enemyData.resist_wind },
            {StatType.LightResist, enemyData.resist_light },
            {StatType.DarkResist, enemyData.resist_dark },
            {StatType.FireDmg, 100 },
            {StatType.WaterDmg, 100 },
            {StatType.IceDmg, 100 },
            {StatType.LightningDmg, 100 },
            {StatType.EarthDmg, 100 },
            {StatType.WindDmg, 100 },
            {StatType.LightDmg, 100 },
            {StatType.DarkDmg, 100 },
         });


        enemy.CurrentHealth = enemy.statBlock.Get(StatType.MaxHealth);

    }

    void InitEnemy(EnemyStats enemy, Tile tile)
    {
        enemy.CurTile = tile;
        enemy.curLevel = tile.curLevel;
        enemy.isDead = false;
        tile.CharacterStatsOnTile = enemy;
        var sr = enemy.GetComponent<SpriteRenderer>();
        sr.sortingOrder = -10 * tile.gridPosition.y;
        //tile.isOccupied = true;
        TurnManager.Instance.AddUnit(enemy.unitBase);
    }

    public void TrySpawnBoss(Level level,QuestData data)
    {
        if(data.main_object_type != ObjectType.DefeatBoss || !level.isLastFloor) return;

        int bossID = GetBossIDByQuestID(data.id); bossID = 230008;
        if (bossID == -1) return;
        int spawnCount = 1;
        Leaf leaf = level.endLeaf;

        List<Tile> availableTiles = new List<Tile>();

        foreach (var pos in TileUtility.GetPositionsInRect(leaf.rect))
        {
            if (level.tiles.TryGetValue(pos, out Tile tile))
            {
                if (tile.tileType == TileType.ground && tile.CharacterStatsOnTile == null)
                {
                    availableTiles.Add(tile);
                }
            }
        }

        for (int i = 0; i < spawnCount && availableTiles.Count > 0; i++)
        {
            Tile targetTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
            availableTiles.Remove(targetTile);

            EnemyStats enemy = enemyPool.GetFromPool(bossID, targetTile.gridPosition, level.transform);

            SetEnemyStat(GetCurEnemyLevel(level), enemy, enemyDataById[bossID]);
            InitEnemy(enemy, targetTile);
        }
    }

    public int GetBossIDByQuestID(int QuestID)
    {
        return QuestID switch
        {
            110004 => 230008,
            _ => -1
        };
    }
}
