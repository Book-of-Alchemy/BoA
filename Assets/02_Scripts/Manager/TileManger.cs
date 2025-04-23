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
    PlayerStats player;

    private void Start()
    {
        SetLevelGenerator(tileData.biomeSet[0], 8, 40, 40);
        curLevel = GenerateLevel();
        PaintLevel(curLevel);
        GameManager.Instance.PlayerTransform.GetComponent<CharacterStats>().curLevel = curLevel;
        foreach (var enemy in GameManager.Instance.Enemies)
        {
            if (enemy == null) continue;

            enemy.curLevel = curLevel;

            if (curLevel.tiles.TryGetValue(new Vector2Int(Mathf.RoundToInt(enemy.transform.position.x), Mathf.RoundToInt(enemy.transform.position.y)), out Tile targerTile))
            {
                targerTile.CharacterStatsOnTile = enemy;
                enemy.curTile = targerTile;
            }
        }
        SpawnPlayer();
    }

    public void SetLevelGenerator(BiomeSet biomeSet, int roomCnt, int rootWidth, int rootHeight)
    {
        levelGenerator.SetLevelGenerator(biomeSet, roomCnt, rootWidth, rootHeight);
    }

    public Level GenerateLevel()
    {
        return levelGenerator.GenerateLevel();
    }

    public void PaintLevel(Level level)
    {
        if (level.isPainted == true) return;

        TilePainter.GenerateTileObject(level, tileData);
    }

    public void GetDownToNextLevel()
    {
        curLevel.gameObject.SetActive(false);
        if (curLevelIndex < levels.Count - 1)
        {
            curLevelIndex++;
            curLevel = levels[curLevelIndex];
            curLevel.gameObject.SetActive(true);
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(curLevel.startTile.gridPosition.x, curLevel.startTile.gridPosition.y, 0);

        if (player == null)
            player = Instantiate(SODataManager.Instance.playerPrefab, spawnPosition, Quaternion.identity).GetComponent<PlayerStats>();
        else
            player.transform.position = spawnPosition;

        player.curLevel = curLevel;
        player.curTile = curLevel.startTile;
    }

    public void CompleteQuest()
    {

    }

    public void FailQuest()
    {

    }
}
