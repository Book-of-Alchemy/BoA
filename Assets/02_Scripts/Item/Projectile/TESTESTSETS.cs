using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTESTSETS : MonoBehaviour
{
    bool isbool = false;
    public BaseItem BaseItem;
    public int id = 200001;
    // Start is called before the first frame update


    public void OnButton()
    {
        if (TestTileManger.Instance.curLevel.tiles.TryGetValue(new Vector2Int(2, 0), out Tile tile))
        {
            ItemManager.Instance.CreateItem(ResourceManager.Instance.dicItemData[id]).UseItem(ResourceManager.Instance.dicItemData[id]);
        }
    }

}
