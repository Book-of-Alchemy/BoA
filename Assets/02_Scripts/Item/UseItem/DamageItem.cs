using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Experimental.GraphView.GraphView;

[System.Serializable]
public class DamageItem : BaseItem
{
    private Tile _curTile;
    public SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    private Tile _choiceTile; // 선택한 타일
    private PlayerStats _player;
    private bool _isObject = false;
    public int dropId;
    public int dropAmount;
    public Action _handler;

    // 초기화
    public void Init(ItemData data)
    {

        _player = GameManager.Instance.PlayerTransform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(_player.CurTile.gridPosition.x, _player.CurTile.gridPosition.y, 0);
        SetType(data);
    }
    private void SetType(ItemData data)
    {
        spriteRenderer.sprite = data.Sprite;
    }

    public override void AddItem(ItemData data)
    {
        if(_curTile.CharacterStatsOnTile is PlayerStats)
        {
            _curTile.itemsOnTile.Remove(this);
            _curTile.onCharacterChanged -= _handler;
            Destroy(this.gameObject);
        }
    }

    public override void DropItem(ItemData data, int amount)
    {
        Init(data);
        dropId = data.id;
        dropAmount = amount;
        _curTile = _player.CurTile;
        _curTile.itemsOnTile.Add(this);
        Debug.Log("아이템 버려짐");
        _handler = () => AddItem(data);
        _curTile.onCharacterChanged += _handler;
    }

    public override void UseItem(ItemData data, Tile tile = null)
    {
        Init(data);
        List<Tile> rangeTiles = CheckRange(data);

        if (tile == null)
            _choiceTile = _player.CurTile;
        else
            _choiceTile = tile;

        if (rangeTiles.Contains(_choiceTile))
            CheckObject(data, _choiceTile);
        else
        {
            Debug.Log("거리범위에 해당하는 타일을 누르지 않았습니다.");
            Destroy(this.gameObject);
        }

        //움직임을 두투윈으로 변경 필요 // oncomplete로 이동이 끝난 후 공격로직을 등록
        if (_isObject)
            transform.DOMove(new Vector3(_choiceTile.gridPosition.x, _choiceTile.gridPosition.y, 0), 1f)
                .SetEase(Ease.Linear)
                .OnComplete(()=> AttackBehavior(data));
        else
        {
            Debug.Log("목표지점 범위에 대상이 없습니다.");
            Destroy(this.gameObject);
        }
    }

    private void AttackBehavior(ItemData data)
    {
        List<Tile> tiles;
        if (data.target_range == 0)
            tiles = TileUtility.GetRangedTile(_player.curLevel, _choiceTile, data.effect_range, false);
        else
            tiles = TileUtility.GetRangedTile(_player.curLevel, _choiceTile, data.effect_range, true);
        foreach (Tile ojTile in tiles)
        {
            if (ojTile.CharacterStatsOnTile != null)
            {
                _player.Attack(ojTile.CharacterStatsOnTile);
            }
        }
        Debug.Log(data.name_en);
        ItemManager.Instance.DestroyRange();
        Destroy(this.gameObject);
    }




    /// <summary>
    /// 선택한 목적지에 적이있는지 확인 로직
    /// </summary>
    public List<Tile> CheckRange(ItemData data)
    {
        List<Tile> checkRangeTiles = new List<Tile>();
        if (data.target_range == 0)
            checkRangeTiles = TileUtility.GetVisibleTiles(_player.curLevel, _player.CurTile, data.target_range);
        else if (data.target_range == 1)
            checkRangeTiles = TileUtility.GetNineTileList(_player.curLevel, _player.CurTile);
        else if (data.target_range >= 2)
            checkRangeTiles = TileUtility.GetVisibleTiles(_player.curLevel, _player.CurTile, data.target_range);

        ItemManager.Instance.CreateRange(checkRangeTiles);

        return checkRangeTiles;
    }
    public void CheckObject(ItemData data, Tile choiceTile)
    {
        List<Tile> checkList = new List<Tile>();
        if (data.effect_range ==1)
            checkList = TileUtility.GetNineTileList(_player.curLevel, choiceTile);
        else if(data.effect_range >= 2 && data.target_range > 0)
            checkList = TileUtility.GetRangedTile(_player.curLevel, choiceTile, data.target_range, true);
        else if(data.effect_range >= 2 && data.target_range == 0)
            checkList = TileUtility.GetRangedTile(_player.curLevel, choiceTile, data.target_range, false);

        //List<Tile> checkList = TileUtility.GetLineTile(_player.curLevel, _player.curTile, _choiceTile, true);
        foreach (Tile tile in checkList)
        {
            if (tile.CharacterStatsOnTile != null)
            {
                _isObject = true;
                break;
            }
        }
    }

}
