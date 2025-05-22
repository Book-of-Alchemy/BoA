using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TileManger : Singleton<TileManger>
{
    public TileDataBase tileData;
    public List<Level> levels = new List<Level>();
    public Level curLevel;
    public int curLevelIndex;
    public int curLevelDepth;
    public LevelGenerator levelGenerator;
    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject environmentalPrefab;
    public static event Action<int> OnGetDown;
    PlayerStats player => GameManager.Instance.PlayerTransform;
    QuestProgress quest = null;
    
    
    //protected void OnEnable()
    //{
    //    GameSceneManager.Instance.OnSceneTypeChanged += GenerateDungeon;

    //}


    //protected void OnDisable()
    //{
    //    GameSceneManager.Instance.OnSceneTypeChanged -= GenerateDungeon;
    //}


    public void GenerateDungeon()
    {
        //if (sceneType != SceneType.Dungeon) return;
        
        curLevelIndex = 0;
        curLevelDepth = 1;
        quest = null;
        if (QuestManager.Instance.AcceptedQuest == null)// 향후 수정 임시코드
        {
            quest = new QuestProgress(SODataManager.Instance.questDataBase.questData[0]);
        }
        else
        {
            quest = QuestManager.Instance.AcceptedQuest;
        }
        levels = levelGenerator.GenerateLevels(7, quest.Data);
        curLevel = levels[curLevelIndex];
        OnGetDown?.Invoke(curLevelIndex);
        SetLevel(curLevel);
        Debug.Log("generate dungeon");
    }


    public void SetLevelGenerator(BiomeSet biomeSet, int roomCnt, int rootWidth, int rootHeight)
    {
        levelGenerator.SetLevelGenerator(biomeSet, roomCnt, rootWidth, rootHeight);
    }

    public Level GenerateLevel()
    {
        return levelGenerator.GenerateLevel();
    }
    public Level GenerateLevel(QuestData questData)
    {
        return levelGenerator.GenerateLevel(7, questData);
    }
    public void SetLevel(Level level)
    {
        PaintLevel(level);
        SpawnEnemy(level);
        SpawnItem(level);
        SpawnPlayer(level);
    }
    public void PaintLevel(Level level)
    {
        if (level.isPainted == true) return;

        TilePainter.GenerateTileObject(level, tileData);
    }

    public void GetDownToNextLevel()
    {
        Level prevLevel = curLevel;
        curLevelIndex++;
        OnGetDown?.Invoke(curLevelIndex);
        if (curLevelIndex <= levels.Count)
        {
            curLevelDepth++;
            TurnManager.Instance.RemoveAllEnemy();
            curLevel = levels[curLevelIndex];
            curLevel.gameObject.SetActive(true);
            SetLevel(curLevel);
            prevLevel.gameObject.SetActive(false);
        }
        else
        {
            EndQuest();
        }
    }

    public void SpawnPlayer(Level level)
    {
        Vector3 spawnPosition = new Vector3(level.startTile.gridPosition.x, level.startTile.gridPosition.y, 0);

        //if (player == null)
        //    player = Instantiate(SODataManager.Instance.playerPrefab, spawnPosition, Quaternion.identity).GetComponent<PlayerStats>();
        //else
            

        player.curLevel = level;
        player.MoveToTile(level.startTile);
        player.transform.position = spawnPosition;
    }

    public void SpawnEnemy(Level level)
    {
        EnemyFactory.Instance.EnemySpawnAtStart(level);
        EnemyFactory.Instance.TrySpawnBoss(level, quest.Data);
    }

    public void SpawnItem(Level level)
    {
        ItemFactory.Instance.ItemSpawnAtStart(level);
    }
    public void EndQuest()
    {
        UIManager.Show<UI_DungeonResult>();
    }

    public void DestroyDungeon()
    {
        if(levels == null) return;  

        foreach(var level in levels)
        {
            Destroy(level.gameObject);
        }
    }
}
