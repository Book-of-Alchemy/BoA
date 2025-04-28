using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public abstract class BaseItem : MonoBehaviour
{
    [Header("공용 아이템 정보")]
    protected SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    protected PlayerStats _player; // 플레이어 정보
    protected ItemData itemData; // 아이템 정보

    [Header("드롭 아이템 정보")]
    protected Tile _curTile; // 드롭된 아이템이 저장될 타일
    public int dropAmount; //
    protected Action _handler;

    public abstract void UseItem(ItemData data);

    /// <summary>
    /// 드롭된 아이템을 추가하는 매서드
    /// </summary>
    /// <param name="data"></param>
    public void AddItem(ItemData data)
    {
        if (_curTile.CharacterStatsOnTile is PlayerStats)
        {
            _curTile.itemsOnTile.Remove(this);
            _curTile.onCharacterChanged -= _handler;
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 아이템을 드롭하는 기능
    /// </summary>
    /// <param name="data">아이템 데이터</param>
    /// <param name="amount"> 아이템 수량</param>
    /// <param name="dropTile">아이템을 드롭하려는 위치, 기본 null로 되어있어저 입력하지 않으면 플레이어 위치로 잡음</param>
    public void DropItem(ItemData data, int amount, Tile dropTile = null)
    {
        Init(data, _curTile);
        dropAmount = amount;
        _curTile.itemsOnTile.Add(this);
        Debug.Log("아이템 버려짐");
        _handler = () => AddItem(data);
        _curTile.onCharacterChanged += _handler;
    }

    /// <summary>
    /// 드롭아이템 정보 및 위치, 스프라이트 초기화
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dropTile"></param>
    public void Init(ItemData data, Tile dropTile)
    {
        _player = GameManager.Instance.PlayerTransform;

        if (dropTile == null)
            _curTile = _player.CurTile;
        else
            _curTile = dropTile;

        itemData = data;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(_curTile.gridPosition.x, _curTile.gridPosition.y + 0.5f, 0);
        SetType(data);
    }

    protected void SetType(ItemData data)
    {
        spriteRenderer.sprite = data.Sprite;
    }

}

