using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTESTSETS : MonoBehaviour
{
    public int id = 200001;
    // Start is called before the first frame update


    public void OnButton()
    {
            ItemManager.Instance.CreateItem(ResourceManager.Instance.dicItemData[id]).UseItem(ResourceManager.Instance.dicItemData[id]);
            ItemManager.Instance.CreateItem(ResourceManager.Instance.dicItemData[id]).DropItem(ResourceManager.Instance.dicItemData[id], 2);
    }

}
