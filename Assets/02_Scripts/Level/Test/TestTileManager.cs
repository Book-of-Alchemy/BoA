using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTileManger : Singleton<TestTileManger>
{
    public TileDataBase tileData;
    public Level curLevel;
    public TestLevelGenerator levelGenerator;
    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject environmentalPrefab;

    private void Start()
    {
        curLevel = GenerateLevel();
        PaintLevel(curLevel);
        GameManager.Instance.PlayerTransform.GetComponent<CharacterStats>().curLevel = curLevel;
        foreach (var enemy in GameManager.Instance.Enemies)
        {
            if (enemy == null) continue;

            enemy.curLevel = curLevel;

            if (curLevel.tiles.TryGetValue(new Vector2Int(Mathf.RoundToInt(enemy.transform.position.x), Mathf.RoundToInt(enemy.transform.position.y)), out Tile targerTile))
            {
                targerTile.characterStats = enemy;
                enemy.curTile = targerTile;
            }
        }


    }


    public Level GenerateLevel()
    {
        return levelGenerator.GenerateTestLevel();
    }

    public void PaintLevel(Level level)
    {
        if (level.isPainted == true) return;

        TestTilePainter.GenerateTileObject(level, tileData);
    }
}

