using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTESTSETS : MonoBehaviour
{
    public GameObject prefab;
    public ItemController controller;
    bool isbool = false;
    // Start is called before the first frame update
    void Start()
    {
        GameObject Prefa = Instantiate(prefab);
        controller = Prefa.GetComponent<ItemController>();
        controller.Init(ResourceManager.Instance.dicItemData[200001]);

    }

    // Update is called once per frame
    void Update()
    {
        if (TestTileManger.Instance.curLevel.tiles.TryGetValue(new Vector2Int(2, 0), out Tile tile) && isbool == false)
        {
            controller.projectileMove.SetDestination(tile);
            controller.projectileMove.Init();
        }
        isbool = true;
    }
}
