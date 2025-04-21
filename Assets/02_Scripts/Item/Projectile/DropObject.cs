using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    private Vector2Int _playerPos;
    private DropItem _dropItem;
    // Start is called before the first frame update
    void Start()
    {
        _dropItem = GetComponent<DropItem>();

        _playerPos = GameManager.Instance.PlayerTransform.curTile.gridPosition;
        transform.position = new Vector3(_playerPos.x, _playerPos.y, 0);

        _dropItem.Item.DropItem();
    }

}
