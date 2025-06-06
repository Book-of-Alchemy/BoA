using System;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    [Header("공용 아이템 정보")]
    public SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    protected PlayerStats _player; // 플레이어 정보
    protected ItemData itemData; // 아이템 정보

    [Header("드롭 아이템 정보")]
    protected Tile _curTile; // 드롭된 아이템이 저장될 타일
    public int dropAmount; //
    protected Action _handler;

    public event Action ItemUseDone;// 아이템 사용이 완전히 끝났을때 발생시킬 이벤트
    public DungeonBehavior dungeonBehavior;

    private void Awake()
    {
        dungeonBehavior = GameManager.Instance.PlayerTransform.GetComponent<DungeonBehavior>();
    }

    public abstract void UseItem(ItemData data);
    public abstract void CancelUse();
    protected void RaiseItemUseDone()
    {
        ItemUseDone?.Invoke();
    }
    protected void FinishUse()
    {
        RaiseItemUseDone();
    }
    /// <summary>
    /// 드롭된 아이템을 추가하는 매서드
    /// </summary>
    /// <param name="data"></param>
    public void AddItem(ItemData data)
    {
        if (_curTile.CharacterStatsOnTile is PlayerStats)
        {
            Inventory.Instance.Add(data,dropAmount);
            _curTile.itemsOnTile.Remove(this);
            _curTile.onCharacterChanged -= _handler;
            _curTile.onIsOnSightChanged -= UpdateVisual;
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
        DropInit(data, dropTile);//테스트를 위하여 일단 수정했습니다...(04.29 이성재)
        dropAmount = amount;
        _curTile.itemsOnTile.Add(this);
        _handler = () => AddItem(data);
        _curTile.onCharacterChanged += _handler;
        _curTile.onIsOnSightChanged += UpdateVisual;
    }

    /// <summary>
    /// 드롭아이템 정보 및 위치, 스프라이트 초기화
    /// 드랍에서만 사용해야함 use시 사용 x
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dropTile"></param>
    public void DropInit(ItemData data, Tile dropTile)
    {
        _player = GameManager.Instance.PlayerTransform;

        if (dropTile == null)
            _curTile = _player.CurTile;
        else
            _curTile = dropTile;

        itemData = data;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
        transform.position = new Vector3(_curTile.gridPosition.x, _curTile.gridPosition.y, 0);
        transform.localScale = new Vector3(2, 2, 1);
        SetType(data);
    }

    protected void SetType(ItemData data)
    {
        spriteRenderer.sprite = data.sprite;
        spriteRenderer.sortingOrder = 1000;
    }
    /// <summary>
    /// 시야에 따른 이미지 처리
    /// </summary>
    public void UpdateVisual()
    {
        if (!spriteRenderer || _curTile == null)
            return;


        Color color;
        bool shouldEnable = true;

        if (_curTile.IsOnSight)
        {
            color = Color.white;
        }
        else if (_curTile.IsExplored)
        {
            color = new Color(0.4f, 0.4f, 0.4f);
        }
        else
        {
            shouldEnable = false;
            color = Color.clear;
        }

        spriteRenderer.enabled = shouldEnable;
        spriteRenderer.color = color;

    }
}

