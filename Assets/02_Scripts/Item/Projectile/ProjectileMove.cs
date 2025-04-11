using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    private ItemController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ItemController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            controller.Item.UseItem();
            Destroy(this.gameObject);
        }
    }
}
