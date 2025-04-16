using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    private ItemController controller;
    private Vector2 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ItemController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += SetNormalizedDir(playerDir) * Time.deltaTime;
    }
    public void SetDir(Vector2 dir)
    {
        playerDir = dir;
    }
    Vector3 SetNormalizedDir(Vector2 dir)
    {
        return dir.normalized;
    }

}
