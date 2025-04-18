using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelGenerator : MonoBehaviour
{
    public Level GenerateTestLevel()
    {
        Level level = new GameObject("Level").AddComponent<Level>();
        level.biomeSet = TestTileManger.Instance.tileData.biomeSet[0];
        level.tileDataBase = TestTileManger.Instance.tileData;

        Vector2Int offSet = new Vector2Int (-7,-5);
        for (int i = 0; i < 14; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                Tile tile = new Tile()
                {
                    gridPosition = new Vector2Int(i,j) +offSet,
                    tileType = TileType.wall,
                    canSeeThrough = false,
                };

                level.tiles[tile.gridPosition] = tile;
            }
        }

        offSet = new Vector2Int (-6,-4);

        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Tile tile = new Tile()
                {
                    gridPosition = new Vector2Int(i, j) + offSet,
                    tileType = TileType.ground,
                    canSeeThrough = true,
                };

                level.tiles[tile.gridPosition] = tile;
            }
        }


        for (int i = -2; i <3;i++)
        {

            Tile tile = new Tile()
            {
                gridPosition = new Vector2Int(0, i),
                tileType = TileType.wall,
                canSeeThrough = false,
            };

            level.tiles[tile.gridPosition] = tile;
        }

        return level;
    }
}
