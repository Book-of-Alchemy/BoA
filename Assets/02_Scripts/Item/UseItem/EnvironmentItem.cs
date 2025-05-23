using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentItem : BaseItem
{
    private List<Tile> rangeTiles = new List<Tile>();
    public float moveSpeed = 5f;
    private void UseInit(ItemData data)
    {
        _player = GameManager.Instance.PlayerTransform;
        itemData = data;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(_player.CurTile.gridPosition.x, _player.CurTile.gridPosition.y + 0.5f, 0);
        transform.localScale = new Vector3(2, 2, 1);
        SetType(data);
    }
    public override void UseItem(ItemData data)
    {
        UseInit(data);
        rangeTiles = CheckRange(data);
        InputManager.Instance.OnMouseMove += CheckEffectRange;
        InputManager.Instance.OnMouseClick += OnClick;
    }
    public override void CancelUse()
    {
        InputManager.Instance.OnMouseMove -= CheckEffectRange;
        InputManager.Instance.OnMouseClick -= OnClick;

        // 화면에 표시된 사거리 지우기
        ItemManager.Instance.DestroyRange();
        ItemManager.Instance.DestroyItemRange();

        // 이 게임 오브젝트 파괴
        Destroy(this.gameObject);
    }
    public List<Tile> CheckRange(ItemData data)
    {
        ItemManager.Instance.DestroyRange();
        List<Tile> checkRangeTiles = new List<Tile>();

        int targetRange = data.target_range;
        if (targetRange == 0)
            checkRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, _player.CurTile, targetRange, true);
        else if (targetRange == 1)
            checkRangeTiles = TileUtility.GetNineVisibleTileList(_player.curLevel, _player.CurTile, true);
        else if (targetRange >= 2)
            checkRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, _player.CurTile, targetRange, true);
        ItemManager.Instance.CreateRange(checkRangeTiles);

        return checkRangeTiles;
    }
    public void CheckEffectRange(Vector3 mousePos)
    {
        ItemManager.Instance.DestroyItemRange();

        Vector2Int mouseWorldPos = Vector2Int.RoundToInt(mousePos);

        _player.curLevel.tiles.TryGetValue(mouseWorldPos, out Tile mouseTile);
        if (rangeTiles.Contains(mouseTile))
        {
            List<Tile> checkItemRangeTiles = new List<Tile>();

            if (itemData.target_range == 0)
            {
                if (itemData.effect_range == 1)
                    checkItemRangeTiles = TileUtility.GetNineVisibleTileList(_player.curLevel, mouseTile, false);
                else
                    checkItemRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, mouseTile, itemData.effect_range, false);
            }
            else
            {
                if (itemData.effect_range == 1)
                    checkItemRangeTiles = TileUtility.GetNineVisibleTileList(_player.curLevel, mouseTile, true);
                else
                    checkItemRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, mouseTile, itemData.effect_range, true);
            }
            ItemManager.Instance.CreateItemRange(checkItemRangeTiles);
        }
    }

    public void OnClick(Vector3 mouseClickPos)
    {
        InputManager.Instance.OnMouseMove -= CheckEffectRange; // 마우스 위치에따라 보여주는 매서드 구독해제
        ItemManager.Instance.DestroyItemRange(); // 아이템 사거리 삭제
        ItemManager.Instance.DestroyRange(); // 아이템 효과 범위 삭제
        InputManager.Instance.OnMouseClick -= OnClick;

        Vector2Int mouseWorldPos = Vector2Int.RoundToInt(mouseClickPos);
        _player.curLevel.tiles.TryGetValue(mouseWorldPos, out Tile mouseTile);

        if (rangeTiles.Contains(mouseTile))
        {
            List<Tile> tiles = TileUtility.GetLineTile(_player.curLevel, _player.CurTile, mouseTile);
            Tile targetTile = mouseTile;
            foreach (Tile objectTile in tiles)
            {
                if (objectTile.CharacterStatsOnTile != null)
                {
                    targetTile = objectTile;
                    break;
                }
            }
            float distance = Vector2Int.Distance(_player.CurTile.gridPosition, targetTile.gridPosition);
            float duration = distance / moveSpeed;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DORotate(new Vector3(0, 0, 360), 0.3f, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLoops(-1));
            seq.Join(transform.DOMove(new Vector3(targetTile.gridPosition.x, targetTile.gridPosition.y + 0.5f, 0), duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (itemData != null)
                        EnvironmentBehavior(itemData, targetTile);
                }
                ));
        }
        else
        {
            CancelUse();
            SubscribeInput();
        }
    }

    private void EnvironmentBehavior(ItemData data, Tile targetTile)
    {
        List<Tile> tiles = new List<Tile>();
        if (data.target_range == 0)
        {
            if (itemData.effect_range == 1)
            {
                tiles = TileUtility.GetNineVisibleTileList(_player.curLevel, targetTile, false);
            }
            else if (itemData.effect_range >= 2)
            {
                tiles = TileUtility.GetItemRangedTile(_player.curLevel, targetTile, itemData.effect_range, false);
            }
        }
        else if (itemData.target_range > 0)
        {
            if (itemData.effect_range == 1)
            {
                tiles = TileUtility.GetNineVisibleTileList(_player.curLevel, targetTile, true);
            }
            else
            {
                tiles = TileUtility.GetItemRangedTile(_player.curLevel, targetTile, itemData.effect_range, true);
            }
        }

        foreach (Tile ojTile in tiles)
        { 
            switch(data.attribute)
            {
                case Attribute.Water:
                    EnvironmentalFactory.Instance.GetEnvironment(EnvironmentType.Shallow_Water, ojTile, _player.curLevel);
                    break;
                case Attribute.Oil:
                    EnvironmentalFactory.Instance.GetEnvironment(EnvironmentType.Oil, ojTile, _player.curLevel);
                    break;
            }
        }


        FinishUse();
        Destroy(this.gameObject, 0.1f);
    }


    public void SubscribeInput()
    {
        var player = GameManager.Instance.PlayerTransform;
        if (player != null)
        {
            var dungeonBehavior = player.GetComponent<DungeonBehavior>();
            if (dungeonBehavior != null)
            {
                dungeonBehavior.SSubscribeInput();
            }
        }
    }
}
