using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTESTSETS : MonoBehaviour
{
    public GameObject prefab;
    public ProjectileItem controller;
    public DropItem controller2;
    public DropItem drop;
    public GameObject dropPrefab;
    // Start is called before the first frame update
    void Start()
    {
        ItemManager.Instance.CreateDropItem(Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<DamageItem>());


    }

}
