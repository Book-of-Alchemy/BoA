using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ItemManager : Singleton<ItemManager>
{
    public GameObject damageItemPrefab;
    public GameObject dropPrefab;
    public GameObject rangeTilePrefab;
    public List<GameObject> rangeTilePrefabs = new List<GameObject> ();
    public GameObject rangeTiles;

    private void Start()
    {
        rangeTiles = new GameObject("RangeTiles");
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
