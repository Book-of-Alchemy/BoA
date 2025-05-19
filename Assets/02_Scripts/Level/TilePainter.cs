using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TilePainter
{
    
    public static void GenerateTileObject(Level level, TileDataBase tileDataBase)
    {
        Dictionary<Vector2Int, Tile> tiles = level.tiles;
        List<GroundTileSet> groundTileSets = level.biomeSet.groundTileSet;
        List<AutoWallTileSet> wallTileSets = level.biomeSet.wallAutoTileSet;
        EnvironmentalDataBase environmentalDataBase = SODataManager.Instance.environmentalDataBase;
        GameObject groundPrefab = TileManger.Instance.groundPrefab;
        GameObject wallPrefab = TileManger.Instance.wallPrefab;
        GameObject environmentalPrefab = TileManger.Instance.environmentalPrefab;
        List<TrapData> trapData = tileDataBase.trapData;


        foreach (var tile in tiles)
        {
            switch (tile.Value.tileType)
            {
                case TileType.ground:
                    SetGroundTile(tile.Value, level, groundTileSets, groundPrefab);
                    break;
                case TileType.wall:
                    SetWallTile(tile.Value, level, wallTileSets, wallPrefab);
                    break;
                default:
                    break;
            }
        }

        foreach (var tile in tiles)
        {
            if (tile.Value.environmentType == EnvironmentType.none)
                continue;

            SetEnvironmentTile(tile.Value, level);
        }

        PlaceTrap(level, trapData);
        PlaceLadder(level, tileDataBase);
    }

    private static void SetGroundTile(Tile tile, Level level, List<GroundTileSet> groundTileSets, GameObject groundPrefab)
    {
        GameObject TileGO = UnityEngine.Object.Instantiate(groundPrefab, new Vector3Int(tile.gridPosition.x, tile.gridPosition.y, 0), Quaternion.identity);
        GroundPrefab tilePrefab = TileGO.GetComponent<GroundPrefab>();
        tilePrefab.CurTile = tile;
        if (tilePrefab.baseRenderer == null) return;

        tilePrefab.baseRenderer.sprite = groundTileSets[0].groundSprite;
        tilePrefab.baseRenderer.sortingOrder = -10000;
        TileGO.transform.SetParent(level.transform);
    }

    private static void SetWallTile(Tile tile, Level level, List<AutoWallTileSet> wallTileSets, GameObject wallPrefab)
    {
        GameObject TileGO = UnityEngine.Object.Instantiate
            (
            wallPrefab,
            new Vector3Int(tile.gridPosition.x, tile.gridPosition.y, 0),
            Quaternion.identity
            );
        GroundPrefab tilePrefab = TileGO.GetComponent<GroundPrefab>();
        tilePrefab.CurTile = tile;
        if (tilePrefab.baseRenderer == null || tilePrefab.upperRenderer == null) return;

        bool isFront = IsFrontWall(tile, level);
        int bitask = CalculateWallBitmask(tile, level);
        tilePrefab.baseRenderer.sprite = wallTileSets[0].GetBaseSprite(bitask, isFront);
        tilePrefab.upperRenderer.sprite = wallTileSets[0].GetUpperSprite(bitask, isFront);
        tilePrefab.baseRenderer.sortingOrder = -tile.gridPosition.y*10;
        tilePrefab.upperRenderer.sortingOrder = -tile.gridPosition.y*10;
        TileGO.transform.SetParent(level.transform);
    }

    private static bool IsFrontWall(Tile tile, Level level)
    {
        Tile down = TileUtility.GetAdjacentTile(level, tile, FourDir.down);
        if (down == null) return false;
        return down.tileType == TileType.ground;
    }

    public static int CalculateWallBitmask(Tile tile, Level level)
    {
        int bitmask = 0;

        // 순서: 상 우 하 좌 


        for (int i = 0; i < 4; i++)
        {
            Tile neighbor = TileUtility.GetAdjacentTile(level, tile, (FourDir)i);
            if (IsWallLike(neighbor))
                bitmask |= 1 << i;
        }

        return bitmask;
    }


    private static bool IsWallLike(Tile tile)
    {
        if (tile == null) return true;

        TileType tileType = tile.tileType;
        return tileType == TileType.wall || tileType == TileType.secretWall || tileType == TileType.empty;
    }

    private static void SetEnvironmentTile(Tile tile, Level level)
    {
        EnvironmentalFactory.Instance.GetEnvironment(tile.environmentType, tile, level);
        /*GameObject TileGO = UnityEngine.Object.Instantiate
            (
            environmentalPrefab,
            new Vector3Int(tile.gridPosition.x, tile.gridPosition.y, 0),
            Quaternion.identity
            );
        
        EnvironmentPrefab tilePrefab = TileGO.GetComponent<EnvironmentPrefab>();
        if (tilePrefab.baseRenderer == null) return;

        int bitask = CalculateEnvironmentBitmask(tile, level, tile.environmentType);
        tilePrefab.baseRenderer.sprite = environmentalSets[0].GetSprite(bitask);
        tilePrefab.baseRenderer.sortingOrder = -9000;
        tilePrefab.CurTile = tile;
        TileGO.transform.SetParent(level.transform);*/
    }

    public static int CalculateEnvironmentBitmask(Tile tile, Level level, EnvironmentType type)
    {
        int bitmask = 0;

        // 순서: 상 우 하 좌 


        for (int i = 0; i < 4; i++)
        {
            Tile neighbor = TileUtility.GetAdjacentTile(level, tile, (FourDir)i);
            if (IsEnvironmentByType(neighbor, type))
                bitmask |= 1 << i;
        }

        return bitmask;
    }
    public static int CalculateEnvironmentBitmaskByTileEffect(Tile tile, Level level, EnvironmentType type)
    {
        int bitmask = 0;

        // 순서: 상 우 하 좌 


        for (int i = 0; i < 4; i++)
        {
            Tile neighbor = TileUtility.GetAdjacentTile(level, tile, (FourDir)i);
            if (IsEnvironmentByTileEffct(neighbor, type))
                bitmask |= 1 << i;
        }

        return bitmask;
    }

    private static bool IsEnvironmentByType(Tile tile, EnvironmentType type)
    {

        EnvironmentType tileType = tile.environmentType;
        return tileType == type;
    }
    private static bool IsEnvironmentByTileEffct(Tile tile, EnvironmentType type)
    {
        if (tile.groundEffect == null)
            return false;
        EnvironmentType tileType = tile.groundEffect.EnvType;
        return tileType == type;
    }
    private static void PlaceTrap(Level level, List<TrapData> trapData)
    {
        List<Tile> tiles = level.trapPoint;

        foreach (var tile in tiles)
        {
            int randomTrap = UnityEngine.Random.Range(0, trapData.Count - 1);
            TrapFactory.CreateTrap(randomTrap,level,tile);
        }
    }

    private static void PlaceLadder(Level level, TileDataBase tileDataBase)
    {
        level.endTile.mapObject = UnityEngine.Object.Instantiate
                (
                tileDataBase.ladder,
                new Vector3Int(level.endTile.gridPosition.x, level.endTile.gridPosition.y, 0),
                Quaternion.identity
                ).GetComponent<MapObject>();

        level.endTile.mapObject.CurTile = level.endTile;
        //level.endTile.mapObject.spriteRenderer.sortingOrder = -level.endTile.gridPosition.y * 10;
    }
}
