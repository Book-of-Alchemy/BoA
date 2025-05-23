
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : Singleton<ItemFactory>
{
    public List<ItemData> itemDatas;
    public Dictionary<int, ItemData> itemdataById;
    public Dictionary<Item_Type, List<ItemData>> dicItemByType;
    public Dictionary<int, RecipeData> recipeDataById;
    public GameObject itemPrefab;

    protected override void Awake()
    {
        base.Awake();
        itemDatas = SODataManager.Instance.itemDataBase.itemDatas;
        itemdataById = SODataManager.Instance.itemDataBase.dicItemData;
        itemPrefab = SODataManager.Instance.itemDataBase.typeObjectPrefab;
        dicItemByType = SODataManager.Instance.itemDataBase.dicItemByType;
        recipeDataById = SODataManager.Instance.itemDataBase.recipeById;
    }
    public void SpawnRecipeItems(Level level)
    {
        List<Leaf> leavesWithoutStart = new List<Leaf>(level.seletedLeaves);
        leavesWithoutStart.Remove(level.startLeaf);

        // 1. 레시피에서 랜덤으로 Leaf 수 만큼 선택
        List<RecipeData> recipePool = new List<RecipeData>(recipeDataById.Values);
        int count = Mathf.Min(recipePool.Count, leavesWithoutStart.Count);

        for (int i = 0; i < count; i++)
        {
            Leaf leaf = leavesWithoutStart[i];
            RecipeData recipe = recipePool[Random.Range(0, recipePool.Count-1)];

            List<Tile> roomTiles = TileUtility.GetRoomTileOnLeaf(level, leaf);
            if (roomTiles.Count == 0) continue;

            // 필요한 재료 ID 리스트
            List<int> materialIds = new List<int>();
            if (recipe.material_1_item_id >= 0) materialIds.Add(recipe.material_1_item_id);
            if (recipe.material_2_item_id >= 0) materialIds.Add(recipe.material_2_item_id);
            if (recipe.material_3_item_id >= 0) materialIds.Add(recipe.material_3_item_id);

            foreach (int materialId in materialIds)
            {
                if (roomTiles.Count == 0) break;
                Tile targetTile = roomTiles[UnityEngine.Random.Range(0, roomTiles.Count)];
                roomTiles.Remove(targetTile);

                if (itemdataById.TryGetValue(materialId, out var itemData))
                {
                    DropItem(itemData, targetTile, 5); // 수량은 필요 시 조정
                }
            }
        }

        // 필요 시 startLeaf에 따로 스폰
        List<Tile> startTiles = TileUtility.GetRoomTileOnLeaf(level, level.startLeaf);
        SpawnItemOnTile(startTiles, 5); // 혹은 원하는 수량
    }


    public void ItemSpawnAtStart(Level level)
    {
        List<Leaf> leavesWithoutStart = new List<Leaf>(level.seletedLeaves);
        leavesWithoutStart.Remove(level.startLeaf);
        
        foreach (var leaf in leavesWithoutStart)
        {
            int spawnCount = GetRandomItemQuntity(leaf);
            if (spawnCount == 0) continue;

            List<Tile> availableTiles = TileUtility.GetRoomTileOnLeaf(level, leaf);

            for (int i = 0; i < spawnCount && availableTiles.Count > 0; i++)
            {
                Tile targetTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
                availableTiles.Remove(targetTile);

                int id = GetRandomItemId(dicItemByType[Item_Type.Material]);
                ItemData itemData = itemdataById[id];
                BaseItem item = DropItem(itemData, targetTile);
                
            }
        }

        List<Tile> startRoomTiles = TileUtility.GetRoomTileOnLeaf(level, level.startLeaf);

        SpawnItemOnTile(startRoomTiles, 5);
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

    void SpawnItemOnTile(List<Tile> availableTiles, int spawnCount)
    {
        availableTiles.Remove(TileManger.Instance.curLevel.startTile);
        List<RecipeData> recipePool = new List<RecipeData>(recipeDataById.Values);

        RecipeData recipe = recipePool[Random.Range(0, recipePool.Count - 1)];

        if (availableTiles.Count == 0) return;

        // 필요한 재료 ID 리스트
        List<int> materialIds = new List<int>();
        if (recipe.material_1_item_id >= 0) materialIds.Add(recipe.material_1_item_id);
        if (recipe.material_2_item_id >= 0) materialIds.Add(recipe.material_2_item_id);
        if (recipe.material_3_item_id >= 0) materialIds.Add(recipe.material_3_item_id);

        foreach (int materialId in materialIds)
        {
            if (availableTiles.Count == 0) break;
            Tile targetTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
            availableTiles.Remove(targetTile);

            if (itemdataById.TryGetValue(materialId, out var itemData))
            {
                DropItem(itemData, targetTile, 5); // 수량은 필요 시 조정
            }
        }
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
    public BaseItem DropItem(ItemData data,Tile targetTile,int quantity = 5)
    {
        GameObject go = new GameObject(data.name_en);
        go.AddComponent<SpriteRenderer>();
        BaseItem item = data.effect_type switch
        {
            Effect_Type.Damage => go.AddComponent<DamageItem>() as BaseItem,
            Effect_Type.Heal => go.AddComponent<HealItem>() as BaseItem,
            Effect_Type.Place_Trap => go.AddComponent<TrapItem>() as BaseItem,
            Effect_Type.Buff => go.AddComponent<BuffItem>() as BaseItem,
            Effect_Type.Debuff => go.AddComponent<DeBuffItem>() as BaseItem,
            Effect_Type.Place_Environment_Tile => go.AddComponent<EnvironmentItem>() as BaseItem,
            _ => go.AddComponent<MaterialItem>()
        };
        item.DropItem(data, quantity, targetTile);
        item.spriteRenderer.sortingOrder = -8000;
        item.transform.SetParent(targetTile.curLevel.transform);

        return item;
    }
    public BaseItem DropItem(int id, Tile targetTile, int quantity = 5)
    {
        if (!itemdataById.TryGetValue(id, out ItemData data))
        {
            return null;
        }
        GameObject go = new GameObject(data.name_en);
        go.AddComponent<SpriteRenderer>();
        BaseItem item = data.effect_type switch
        {
            Effect_Type.Damage => go.AddComponent<DamageItem>() as BaseItem,
            Effect_Type.Heal => go.AddComponent<HealItem>() as BaseItem,
            Effect_Type.Place_Trap => go.AddComponent<TrapItem>() as BaseItem,
            Effect_Type.Buff => go.AddComponent<BuffItem>() as BaseItem,
            Effect_Type.Debuff => go.AddComponent<DeBuffItem>() as BaseItem,
            Effect_Type.Place_Environment_Tile => go.AddComponent<EnvironmentItem>() as BaseItem,
            _ => go.AddComponent<MaterialItem>()
        };
        item.DropItem(data, quantity, targetTile);
        item.spriteRenderer.sortingOrder = -8000;
        item.transform.SetParent(targetTile.curLevel.transform);

        return item;
    }

    public BaseItem CreateItem(int id)
    {
        if (!itemdataById.TryGetValue(id, out ItemData data))
        {
            return null;
        }
        GameObject go = new GameObject(data.name_en);
        go.AddComponent<SpriteRenderer>();
        BaseItem item = data.effect_type switch
        {
            Effect_Type.Damage => go.AddComponent<DamageItem>() as BaseItem,
            Effect_Type.Heal => go.AddComponent<HealItem>() as BaseItem,
            Effect_Type.Place_Trap => go.AddComponent<TrapItem>() as BaseItem,
            Effect_Type.Buff => go.AddComponent<BuffItem>() as BaseItem,
            Effect_Type.Debuff => go.AddComponent<DeBuffItem>() as BaseItem,
            Effect_Type.Place_Environment_Tile => go.AddComponent<EnvironmentItem>() as BaseItem,
            _ => go.AddComponent<MaterialItem>()
        };
        go.GetComponent<SpriteRenderer>().sortingOrder = -8000;

        return item;
    }
}
