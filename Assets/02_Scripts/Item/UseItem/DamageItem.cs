using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]
public class DamageItem : BaseItem
{
    Tile curTile;
    public override void AddItem()
    {
        if(curTile.CharacterStatsOnTile is PlayerStats)
        {
            curTile.itemsOnTile.Remove(this);
            curTile.onCharacterChanged -= AddItem;
            Destroy(this.gameObject);
        }
    }

    public override void DropItem()
    {
        curTile = GameManager.Instance.PlayerTransform.curTile;
        curTile.itemsOnTile.Add(this);
        Debug.Log("아이템 버려짐");
        curTile.onCharacterChanged += AddItem;
    }

    /// <summary>
    /// 타일 범위 확인 후 공격 
    /// </summary>
    public override void UseItem()
    {
        //List<Tile> tiles;
        //if (data.target_range == 0 && data.effect_type == Effect_Type.Damage)
        //    tiles = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, 목적지, data.effect_range, false);
        //else
        //    tiles = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, 목적지, data.effect_range, true);
        //foreach (Tile tile in tiles)
        //{
        //    if (tile.characterStats != null)
        //    {
        //        GameManager.Instance.PlayerTransform.Attack(tile.characterStats);
        //    }
        //}

    }

}
