using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]
public class DamageItem : BaseItem
{
    private Tile _curTile;
    private ItemData _data;
    private Action _handler;
    public GameObject prefab;
    public ProjectileItem controller;

    public override void AddItem(ItemData data)
    {
        if(_curTile.CharacterStatsOnTile is PlayerStats)
        {
            _curTile.itemsOnTile.Remove(this.gameObject.GetComponent<DropItem>());
            _curTile.onCharacterChanged -= _handler;
            Destroy(this.gameObject);
        }
    }

    public override void DropItem(ItemData data)
    {
        _curTile = GameManager.Instance.PlayerTransform.curTile;
        _curTile.itemsOnTile.Add(this.gameObject.GetComponent<DropItem>());
        Debug.Log("아이템 버려짐");
        _data = data;
        _handler = () => AddItem(_data);
        _curTile.onCharacterChanged += _handler;
    }

    /// <summary>
    /// 타일 범위 확인 후 공격 
    /// </summary>
    public override void UseItem(ItemData data)
    {
        //GameObject Prefa = Instantiate(prefab);
        //controller = Prefa.GetComponent<ProjectileItem>();
        //controller.Init(ResourceManager.Instance.dicItemData[200002], Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<DamageItem>());
        //if(TestTileManger.Instance.curLevel.tiles.TryGetValue(new Vector2Int(2, 0), out Tile tile))
        //{
        //    controller.projectileMove.SetDestination(tile);
        //    controller.projectileMove.Init();
        //}

        Vector2Int curPos = Vector2Int.RoundToInt(transform.position);
        List<Tile> tiles;
        if (data.target_range == 0 && data.effect_type == Effect_Type.Damage)
            tiles = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, GameManager.Instance.PlayerTransform.curLevel.tiles[curPos], data.effect_range, false);
        else
            tiles = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, GameManager.Instance.PlayerTransform.curLevel.tiles[curPos], data.effect_range, true);
        foreach (Tile tile in tiles)
        {
            if (tile.CharacterStatsOnTile != null)
            {
                GameManager.Instance.PlayerTransform.Attack(tile.CharacterStatsOnTile);
            }
        }
        Debug.Log(data.name_en);

    }

}
