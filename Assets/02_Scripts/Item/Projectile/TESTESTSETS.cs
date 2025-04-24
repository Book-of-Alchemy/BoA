using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTESTSETS : MonoBehaviour
{
    bool isbool = false;
    public BaseItem BaseItem;
    public int id = 200001;
    // Start is called before the first frame update
    void Start()
    {
       
    }
    private void Update()
    {
        if (TestTileManger.Instance.curLevel.tiles.TryGetValue(new Vector2Int(2, 0), out Tile tile) && !isbool)
        { 
            ItemManager.Instance.CreateItem(ResourceManager.Instance.dicItemData[200001]).DropItem(ResourceManager.Instance.dicItemData[200001],2);
            ItemManager.Instance.CreateItem(ResourceManager.Instance.dicItemData[200002]).UseItem(ResourceManager.Instance.dicItemData[200001],tile);
            isbool = true;
        }
    }

    public void OnButton()
    {
        if (TestTileManger.Instance.curLevel.tiles.TryGetValue(new Vector2Int(2, 0), out Tile tile))
        {
            ItemManager.Instance.CreateItem(ResourceManager.Instance.dicItemData[id]).UseItem(ResourceManager.Instance.dicItemData[id], tile);
        }
    }

}
