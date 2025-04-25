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
    public ItemData itemData;
    public int dropAmount;
    public Action _handler;
    private List<Tile> rangeTiles = new List<Tile>();
    private Tile mouseClickTile;

    // 초기화
    public void Init(ItemData data)
    {

        _player = GameManager.Instance.PlayerTransform;
        itemData = data;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(_player.CurTile.gridPosition.x, _player.CurTile.gridPosition.y + 0.5f, 0);
        SetType(data);
    }
    private void SetType(ItemData data)
    {
        spriteRenderer.sprite = data.Sprite;
    }

    public override void AddItem(ItemData data)
    {
        if (_curTile.CharacterStatsOnTile is PlayerStats)
        {
            _curTile.itemsOnTile.Remove(this);
            _curTile.onCharacterChanged -= _handler;
            Destroy(this.gameObject);
        }
    }

    public override void DropItem(ItemData data, int amount)
    {
        Init(data);
        dropAmount = amount;
        _curTile = _player.CurTile;
        _curTile.itemsOnTile.Add(this);
        Debug.Log("아이템 버려짐");
        _handler = () => AddItem(data);
        _curTile.onCharacterChanged += _handler;
    }

    public override void UseItem(ItemData data)
    {
        Init(data);
        rangeTiles = CheckRange(data);
        InputManager.Instance.EnableMouseTracking = true;
        InputManager.Instance.OnMouseMove += CheckEffectRange;
        InputManager.Instance.OnMouseClick += OnClick;

        //if (tile == null)
        //    _choiceTile = _player.CurTile;
        //else
        //    _choiceTile = tile;
        //

    }

    public void OnClick(Vector3 mouseClickPos)
    {
        InputManager.Instance.OnMouseMove -= CheckEffectRange;
        ItemManager.Instance.DestroyItemRange();
        ItemManager.Instance.DestroyRange();

        Vector2Int mouseWorldPos = Vector2Int.RoundToInt(mouseClickPos);
        TestTileManger.Instance.curLevel.tiles.TryGetValue(mouseWorldPos, out mouseClickTile);

        if (rangeTiles.Contains(mouseClickTile))
            CheckObject(itemData, mouseClickTile);
        else
        {
            Debug.Log("거리범위에 해당하는 타일을 누르지 않았습니다.");
            InputManager.Instance.EnableMouseTracking = false;
            InputManager.Instance.OnMouseClick -= OnClick;
            Destroy(this.gameObject);
        }

        //움직임을 두투윈으로 변경 필요 // oncomplete로 이동이 끝난 후 공격로직을 등록
        if (_isObject)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLoops(-1));
            seq.Join(transform.DOMove(new Vector3(mouseClickTile.gridPosition.x, mouseClickTile.gridPosition.y + 0.5f, 0), 1f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (itemData != null)
                        AttackBehavior(itemData);
                    else
                        Debug.Log("데이터가 null");
                }
                ));
        }
        else
        {
            Debug.Log("목표지점 범위에 대상이 없습니다.");
            InputManager.Instance.OnMouseClick -= OnClick;
            InputManager.Instance.EnableMouseTracking = false;
            Destroy(this.gameObject);
        }
    }

    private void AttackBehavior(ItemData data)
    {
        List<Tile> tiles = new List<Tile>();
        if (data.target_range == 0)
        {
            if (data.effect_range == 1)
            {
                tiles = TileUtility.GetAdjacentTileList(_player.curLevel, mouseClickTile, true);
            }
            else if (data.effect_range >= 2)
            {
                tiles = TileUtility.GetItemRangedTile(_player.curLevel, mouseClickTile, data.effect_range, false);
            }
        }
        else if (data.target_range > 0)
        {
            if (data.effect_range == 1)
            {
                tiles = TileUtility.GetNineTileList(_player.curLevel, mouseClickTile);
            }
            else
            {
                tiles = TileUtility.GetItemRangedTile(_player.curLevel, mouseClickTile, data.effect_range, true);
            }
        }
        foreach (Tile ojTile in tiles)
        {
            if (ojTile.CharacterStatsOnTile != null)
            {
                _player.Attack(ojTile.CharacterStatsOnTile);
            }
        }
        Debug.Log(data.name_en);
        InputManager.Instance.EnableMouseTracking = false;
        InputManager.Instance.OnMouseClick -= OnClick;
        Destroy(this.gameObject);
    }

    public void CheckEffectRange(Vector3 mousePos)
    {
        ItemManager.Instance.DestroyItemRange();

        Vector2Int mouseWorldPos = Vector2Int.RoundToInt(mousePos);

        TestTileManger.Instance.curLevel.tiles.TryGetValue(mouseWorldPos, out Tile mouseTile);
        if (rangeTiles.Contains(mouseTile))
        {
            List<Tile> checkItemRangeTiles = new List<Tile>();
            if (itemData.effect_range == 0)
                checkItemRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, mouseTile, itemData.effect_range);
            else if (itemData.effect_range == 1)
                checkItemRangeTiles = TileUtility.GetNineTileList(_player.curLevel, mouseTile);
            else if (itemData.effect_range >= 2)
                checkItemRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, mouseTile, itemData.effect_range);

            ItemManager.Instance.CreateItemRange(checkItemRangeTiles);
        }
    }

    /// <summary>
    /// 사거리 확인용
    /// </summary>
    public List<Tile> CheckRange(ItemData data)
    {
        List<Tile> checkRangeTiles = new List<Tile>();
        if (data.target_range == 0)
            checkRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, _player.CurTile, data.target_range);
        else if (data.target_range == 1)
            checkRangeTiles = TileUtility.GetNineTileList(_player.curLevel, _player.CurTile);
        else if (data.target_range >= 2)
            checkRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, _player.CurTile, data.target_range, true);

        ItemManager.Instance.CreateRange(checkRangeTiles);

        return checkRangeTiles;
    }

    /// <summary>
    /// 범위 안에 오브젝트 있는지 확인
    /// </summary>
    /// <param name="data"></param>
    /// <param name="choiceTile"></param>
    public void CheckObject(ItemData data, Tile choiceTile)
    {
        List<Tile> checkList = new List<Tile>();
        if (data.effect_range == 1)
            checkList = TileUtility.GetNineTileList(_player.curLevel, choiceTile);
        else if (data.effect_range >= 2 && data.target_range > 0)
            checkList = TileUtility.GetItemRangedTile(_player.curLevel, choiceTile, data.effect_range, true);
        else if (data.effect_range >= 2 && data.target_range == 0)
            checkList = TileUtility.GetItemRangedTile(_player.curLevel, choiceTile, data.effect_range, false);

        //List<Tile> checkList = TileUtility.GetLineTile(_player.curLevel, _player.curTile, _choiceTile, true);
        foreach (Tile tile in checkList)
        {
            if (tile.CharacterStatsOnTile != null)
            {
                _isObject = true;
            }
        }
    }

}
