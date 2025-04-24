using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ItemManager : Singleton<ItemManager>
{
    public GameObject projectileItemprefab;
    public GameObject dropPrefab;
    public ProjectileItem projectileItem;
    public DropItem dropItem;


    public void CreateProjectileItem(ItemData data)
    {
        GameObject Prefa = Instantiate(projectileItemprefab);
        projectileItem = Prefa.GetComponent<ProjectileItem>();
        if (TestTileManger.Instance.curLevel.tiles.TryGetValue(new Vector2Int(2, 0), out Tile tile))
            projectileItem.Init(data, tile);
    }

    public void CreateDropItem(ItemData data, BaseItem itemType)
    {
        GameObject DropPre = Instantiate(dropPrefab);
        dropItem = DropPre.GetComponent<DropItem>();
        dropItem.Init(data, itemType);
    }
}
