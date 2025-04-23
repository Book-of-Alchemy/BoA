using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        SetLevelGenerator(tileData.biomeSet[0], 8, 40, 40);
        curLevel = GenerateLevel();
        PaintLevel(curLevel);
        GameManager.Instance.PlayerTransform.GetComponent<CharacterStats>().curLevel = curLevel;
        foreach( var enemy in GameManager.Instance.Enemies)
        {
            if (enemy == null) continue;

            enemy.curLevel = curLevel;

            if (curLevel.tiles.TryGetValue(new Vector2Int(Mathf.RoundToInt(enemy.transform.position.x), Mathf.RoundToInt(enemy.transform.position.y)), out Tile targerTile))
            {
                targerTile.CharacterStatsOnTile = enemy;
                enemy.curTile = targerTile;
            }
        }
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
        }
    }

    public void CompleteQuest()
    {

    }

    public void FailQuest()
    {

    }
}
