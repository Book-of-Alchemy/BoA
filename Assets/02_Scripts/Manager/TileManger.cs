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
    public LevelGenerator levelGenerator;
    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject environmentalPrefab;
    public static event Action<int> OnGetDown;
    PlayerStats player => GameManager.Instance.PlayerTransform;

    protected override void Awake()
    {
        base.Awake();
        GameSceneManager.Instance.OnSceneTypeChanged += GenerateDungeon;
    }

    private void Start()//임시코드
    {
        //SetLevelGenerator(tileData.biomeSet[0], 8, 40, 40);
        //curLevel = GenerateLevel();
        //GenerateDungeon(SceneType.Dungeon);
    }

    private void OnDestroy()
    {
        GameSceneManager.Instance.OnSceneTypeChanged -= GenerateDungeon;
    }

    public void GenerateDungeon(SceneType sceneType)
    {
        if (sceneType != SceneType.Dungeon) return;
        curLevelIndex = 0;
        QuestProgress quest = null;
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
        if (curLevelIndex < levels.Count)
        {
            TurnManager.Instance.RemoveAllEnemy();
            curLevel = levels[curLevelIndex];
            curLevel.gameObject.SetActive(true);
            SetLevel(curLevel);
            prevLevel.gameObject.SetActive(false);
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
    }

    public void SpawnItem(Level level)
    {
        ItemFactory.Instance.ItemSpawnAtStart(level);
    }
    public void CompleteQuest()
    {
        
    }

    public void FailQuest()
    {

    }
}
