using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]
public class DamageItem : BaseItem
{
    private Tile _curTile;

    public override void AddItem(ItemData data)
    {
        if(_curTile.CharacterStatsOnTile is PlayerStats)
        {
            _curTile.itemsOnTile.Remove(this.gameObject.GetComponent<BaseItem>());
            _curTile.onCharacterChanged -= this.gameObject.GetComponent<DropItem>()._handler;
            Destroy(this.gameObject);
        }
    }

    public override void DropItem(ItemData data)
    {
        // 드랍 아이템 생성
        ItemManager.Instance.CreateDropItem(data,this);
    }

    /// <summary>
    /// 타일 범위 확인 후 공격 
    /// </summary>
    public override void UseItem(ItemData data)
    {
        ItemManager.Instance.CreateProjectileItem(data);

        //Vector2Int curPos = Vector2Int.RoundToInt(transform.position);
        //List<Tile> tiles;
        //if (data.target_range == 0 && data.effect_type == Effect_Type.Damage)
        //    tiles = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, GameManager.Instance.PlayerTransform.curLevel.tiles[curPos], data.effect_range, false);
        //else
        //    tiles = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, GameManager.Instance.PlayerTransform.curLevel.tiles[curPos], data.effect_range, true);
        //foreach (Tile tile in tiles)
        //{
        //    if (tile.CharacterStatsOnTile != null)
        //    {
        //        GameManager.Instance.PlayerTransform.Attack(tile.CharacterStatsOnTile);
        //    }
        //}
        //Debug.Log(data.name_en);

    }

}
