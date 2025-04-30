using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : Singleton<ItemFactory>
{
    public List<ItemData> itemDatas;
    public Dictionary<int, ItemData> itemdataById;
    public GameObject itemPrefab;

    protected override void Awake()
    {
        base.Awake();
        itemDatas = SODataManager.Instance.itemDataBase.itemDatas;
        itemdataById = SODataManager.Instance.itemDataBase.dicItemData;
        itemPrefab = SODataManager.Instance.itemDataBase.typeObjectPrefab;
    }
    public void ItemSpawnAtStart(Level level)
    {
        List<Leaf> leavesWithoutStart = new List<Leaf>(level.seletedLeaves);
        leavesWithoutStart.Remove(level.startLeaf);


        foreach (var leaf in leavesWithoutStart)
        {
            int spawnCount = GetRandomItemQuntity(leaf);
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

                int id = GetRandomItemId(itemDatas);
                ItemData itemData = itemdataById[id];
                BaseItem item = SetComponentOnItem(itemData);
                item.DropItem(itemData, 5, targetTile);
            }
        }
    }

    int GetRandomItemQuntity(Leaf leaf)
    {
        int result = leaf.room.roomSizeType switch
        {
            RoomSizeType.small => SmallRoomQuantity(),
            RoomSizeType.medium => MediumRoomQuantity(),
            RoomSizeType.large => LargeRoomQuantity(),
            _ => 0,
        };

        return result;
    }

    int SmallRoomQuantity()
    {
        float rand = Random.value;
        if (rand < 0.5f)
            return 2;

        return 3;
    }
    int MediumRoomQuantity()
    {
        float rand = Random.value;
        if (rand < 0.5f)
            return 3;

        return 4;
    }
    int LargeRoomQuantity()
    {
        float rand = Random.value;
        if (rand < 0.33f)
            return 4;
        else if (rand < 0.66f)
            return 5;

        return 6;
    }

    // getproper item 으로 수정 예정 현재 태그 없음
    /*List<EnemyData> GetProperEnemies(Level level)
    {
        if (!enemyDataByBiome.ContainsKey(level.biomeSet.id))
            return null;

        List<EnemyData> enemyDatas = new List<EnemyData>(enemyDataByBiome[level.biomeSet.id]);
        int curEnemyLevel = GetCurEnemyLevel(level);
        enemyDatas.RemoveAll(enemy => curEnemyLevel > enemy.max_level);
        enemyDatas.RemoveAll(enemy => curEnemyLevel < enemy.min_level);

        return enemyDatas;
    }*/



    int GetRandomItemId(List<ItemData> item)
    {
        return item[UnityEngine.Random.Range(0, item.Count - 1)].id;
    }
    BaseItem SetComponentOnItem(ItemData data)
    {
        GameObject go = new GameObject(data.name_en);
        go.AddComponent<SpriteRenderer>();
        BaseItem item = data.effect_type switch
        {
            Effect_Type.Damage => go.AddComponent<DamageItem>() as BaseItem,
            _ => go.AddComponent<MaterialItem>()
        };
        item.spriteRenderer = item.GetComponent<SpriteRenderer>();


        return item;
    }

}
