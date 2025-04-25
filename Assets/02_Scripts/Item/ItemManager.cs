using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ItemManager : Singleton<ItemManager>
{
    public GameObject damageItemPrefab; // 데미지 아이템 프리팹
    public GameObject rangeTilePrefab; // 사거리 표시 프리팹
    public GameObject itemRangeTilePrefab; // 아이템 효과범위 프리팹
    public List<GameObject> rangeTilePrefabs = new List<GameObject> (); // 생성된 사거리 표시 프리팹들리스트
    public List<GameObject> itemRangeTilePrefabs = new List<GameObject> (); // 생성된 효과범위 프리팹들리스트
    public GameObject rangeTiles; // 사거리 표시 오브젝트들을 넣기위해 만든 빈오브젝트
    public GameObject itemRangeTiles; // 효과범위 오브젝트들을 넣기 위해 만든 빈 오브젝트

    private void Start()
    {
        rangeTiles = new GameObject("RangeTiles");
        itemRangeTiles = new GameObject("ItemRangeTiles");
    }

    public BaseItem CreateItem(ItemData data)
    {
        BaseItem item = null;
        switch (data.effect_type)
        {
            case Effect_Type.Damage:
               item =Instantiate(damageItemPrefab).GetComponent<DamageItem>(); break;
            //case Effect_Type.Heal:
            //    item = Instantiate(projectileItemprefab).GetComponent<HealItem>(); break;
            //case Effect_Type.Buff:
            //    item = Instantiate(projectileItemprefab).GetComponent<BuffItem>(); break;
            //case Effect_Type.Debuff:
            //    item = Instantiate(projectileItemprefab).GetComponent<DeBuffItem>(); break;
            //case Effect_Type.Move:
            //    item = Instantiate(projectileItemprefab).GetComponent<MoveItem>(); break;
        }
        return item;
    }

    public void CreateRange(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            rangeTilePrefabs.Add(Instantiate(rangeTilePrefab, new Vector3(tile.gridPosition.x, tile.gridPosition.y, 0), Quaternion.identity, rangeTiles.transform));
        }
    }

    public void DestroyRange()
    { 
        foreach(GameObject rangeObject in rangeTilePrefabs)
        {
            Destroy(rangeObject);
        }
        rangeTilePrefabs.Clear();
    }

    public void CreateItemRange(List<Tile> tiles)
    {
        foreach(Tile tile in tiles)
        {
            itemRangeTilePrefabs.Add(Instantiate(itemRangeTilePrefab, new Vector3(tile.gridPosition.x, tile.gridPosition.y, 0), Quaternion.identity, itemRangeTiles.transform));
        }
    }

    public void DestroyItemRange()
    {
        foreach (GameObject itemRangeObject in itemRangeTilePrefabs)
        {
            Destroy(itemRangeObject);
        }
        itemRangeTilePrefabs.Clear();
    }



    //public void CreateProjectileItem(ItemData data)
    //{
    //    GameObject Prefa = Instantiate(projectileItemprefab);
    //    projectileItem = Prefa.GetComponent<ProjectileItem>();
    //    if (TestTileManger.Instance.curLevel.tiles.TryGetValue(new Vector2Int(2, 0), out Tile tile))
    //        projectileItem.Init(data, tile);
    //}

    //public void CreateDropItem(ItemData data, BaseItem itemType)
    //{
    //    GameObject DropPre = Instantiate(dropPrefab);
    //    dropItem = DropPre.GetComponent<DropItem>();
    //    dropItem.Init(data, itemType);
    //}
}
