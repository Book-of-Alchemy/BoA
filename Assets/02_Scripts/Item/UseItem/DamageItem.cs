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
    [Header("사용 아이템 정보")]
    private bool _isObject;
    private List<Tile> rangeTiles = new List<Tile>();
    private Tile mouseClickTile;


    /// <summary>
    /// UseItem 위치, 정보, 스프라이트 초기화
    /// </summary>
    /// <param name="data"></param>
    public void Init(ItemData data)
    {
        _player = GameManager.Instance.PlayerTransform;
        itemData = data;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(_player.CurTile.gridPosition.x, _player.CurTile.gridPosition.y + 0.5f, 0);
        SetType(data);
        _isObject = false;
    }

    /// <summary>
    /// 아이템 사용 매서드
    /// </summary>
    /// <param name="data"></param>
    public override void UseItem(ItemData data)
    {
        Init(data);
        rangeTiles = CheckRange(data);
        InputManager.Instance.EnableMouseTracking = true;
        InputManager.Instance.OnMouseMove += CheckEffectRange;
        InputManager.Instance.OnMouseClick += OnClick;
    }

    /// <summary>
    /// 아이템 사용 후 사용하려는 위치에 마우스를 눌렀을때 동작하는 매서드
    /// </summary>
    /// <param name="mouseClickPos"></param>
    public void OnClick(Vector3 mouseClickPos)
    {
        InputManager.Instance.OnMouseMove -= CheckEffectRange; // 마우스 위치에따라 보여주는 매서드 구독해제
        ItemManager.Instance.DestroyItemRange(); // 아이템 사거리 삭제
        ItemManager.Instance.DestroyRange(); // 아이템 효과 범위 삭제

        Vector2Int mouseWorldPos = Vector2Int.RoundToInt(mouseClickPos);
        TestTileManger.Instance.curLevel.tiles.TryGetValue(mouseWorldPos, out mouseClickTile);

        // 사거리 내에 타일을 클릭했는지 확인하는 조건, 사거리 내의 타일을 클릭했다면 효과범위내에 대상들이 있는지 확인
        if (rangeTiles.Contains(mouseClickTile))
            CheckObject(itemData, mouseClickTile);
        else
        {
            Debug.Log("거리범위에 해당하는 타일을 누르지 않았습니다.");
            InputManager.Instance.EnableMouseTracking = false;
            InputManager.Instance.OnMouseClick -= OnClick;
            Destroy(this.gameObject);
        }

        // 범위내에 대상들이 있다면 이동 및 공격동작
        if (_isObject)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DORotate(new Vector3(0, 0, 360), 1f/TurnManager.Instance.turnSpeed, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLoops(-1));
            seq.Join(transform.DOMove(new Vector3(mouseClickTile.gridPosition.x, mouseClickTile.gridPosition.y + 0.5f, 0), 1f/ TurnManager.Instance.turnSpeed)
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

    /// <summary>
    /// 공격 로직
    /// </summary>
    /// <param name="data"></param>
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

    /// <summary>
    /// 아이템 사용시 아이템 효과 범위를 마우스 움직임에 따라 실시간으로 보여주는 매서드
    /// </summary>
    /// <param name="mousePos"></param>
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
    /// 아이템 사용시 클릭 가능한 아이템 사거리를 표시해주고 타일리스트를 반환해주는 매서드
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
