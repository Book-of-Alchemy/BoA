using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManger : Singleton<TileManger>
{
    public TileDataBase tileData;
    public Level curLevel;
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
}
