using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ItemManager : Singleton<ItemManager>
{
    public GameObject rangeTilePrefab; // 사거리 표시 프리팹
    public GameObject itemRangeTilePrefab; // 아이템 효과범위 프리팹
    public GameObject bossRangeTilePrefab; // 보스 범위 프리팹
    public List<GameObject> rangeTilePrefabs = new List<GameObject>(); // 생성된 사거리 표시 프리팹들리스트
    public List<GameObject> itemRangeTilePrefabs = new List<GameObject>(); // 생성된 효과범위 프리팹들리스트
    public Dictionary<string, List<GameObject>> bossRangeTilePrefabs = new (); // 생성된 보스범위 프리팹리스트
    public GameObject rangeTiles; // 사거리 표시 오브젝트들을 넣기위해 만든 빈오브젝트
    public GameObject itemRangeTiles; // 효과범위 오브젝트들을 넣기 위해 만든 빈 오브젝트
    public GameObject bossRangeTiles;

    private void Start()
    {
        rangeTiles = new GameObject("RangeTiles");
        itemRangeTiles = new GameObject("ItemRangeTiles");
        bossRangeTiles = new GameObject("BossRangeTiles");
    }

    //public BaseItem CreateItem(ItemData data)
    //{
    //    BaseItem item = null;
    //    switch (data.effect_type)
    //    {
    //        case Effect_Type.Damage:
    //           item =Instantiate(damageItemPrefab).GetComponent<DamageItem>(); break;
    //        //case Effect_Type.Heal:
    //        //    item = Instantiate(projectileItemprefab).GetComponent<HealItem>(); break;
    //        //case Effect_Type.Buff:
    //        //    item = Instantiate(projectileItemprefab).GetComponent<BuffItem>(); break;
    //        //case Effect_Type.Debuff:
    //        //    item = Instantiate(projectileItemprefab).GetComponent<DeBuffItem>(); break;
    //        //case Effect_Type.Move:
    //        //    item = Instantiate(projectileItemprefab).GetComponent<MoveItem>(); break;
    //    }
    //    return item;
    //}

    public void CreateRange(List<Tile> tiles)
    {
        if (tiles.Count > rangeTilePrefabs.Count)
        {
            int count = rangeTilePrefabs.Count;
            for (int i = 0; i < tiles.Count - count; i++)
            {
                rangeTilePrefabs.Add(Instantiate(rangeTilePrefab, rangeTiles.transform));
            }
        }
        for (int i = 0; i < tiles.Count; i++)
        {
            rangeTilePrefabs[i].transform.position = new Vector3(tiles[i].gridPosition.x, tiles[i].gridPosition.y, 0);
            rangeTilePrefabs[i].SetActive(true);
        }
    }

    public void DestroyRange()
    {
        foreach (GameObject rangeObject in rangeTilePrefabs)
        {
            rangeObject.SetActive(false);
        }
    }

    public void CreateItemRange(List<Tile> tiles)
    {
        if (tiles.Count > itemRangeTilePrefabs.Count)
        {
            int count = itemRangeTilePrefabs.Count;
            for (int i = 0; i < tiles.Count - count; i++)
            {
                itemRangeTilePrefabs.Add(Instantiate(itemRangeTilePrefab, itemRangeTiles.transform));
            }
        }
        for (int i = 0; i < tiles.Count; i++)
        {
            itemRangeTilePrefabs[i].transform.position = new Vector3(tiles[i].gridPosition.x, tiles[i].gridPosition.y, 0);
            itemRangeTilePrefabs[i].SetActive(true);
        }
    }

    public void DestroyItemRange()
    {
        foreach (GameObject itemRangeObject in itemRangeTilePrefabs)
        {
            itemRangeObject.SetActive(false);
        }
    }

    public void CreateBossRange(List<Tile> tiles, string objectName)
    {
        if (tiles.Count > bossRangeTilePrefabs.Count)
        {
            int count = bossRangeTilePrefabs.Count;
            if (!bossRangeTilePrefabs.ContainsKey(objectName))
                bossRangeTilePrefabs[objectName] = new List<GameObject>();
            for (int i = 0; i < tiles.Count - count; i++)
            {
                bossRangeTilePrefabs[objectName].Add(Instantiate(bossRangeTilePrefab, bossRangeTiles.transform));
            }
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            bossRangeTilePrefabs[objectName][i].transform.position = new Vector3(tiles[i].gridPosition.x, tiles[i].gridPosition.y, 0);
            bossRangeTilePrefabs[objectName][i].SetActive(true);
        }
    }

    public void DestroyBossRange(string objectName)
    {
        if (!bossRangeTilePrefabs.ContainsKey(objectName)) return;

        foreach (GameObject BossRangeObject in bossRangeTilePrefabs[objectName])
        {
            BossRangeObject.SetActive(false);
        }
    }


}
