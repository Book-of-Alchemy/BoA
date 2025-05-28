
using System.Collections.Generic;
using UnityEngine;

public class DeBuffItem : BaseItem
{
    private List<Tile> rangeTiles = new List<Tile>();
    private void UseInit(ItemData data)
    {
        itemData = data;
        _player = GameManager.Instance.PlayerTransform;
    }
    public override void UseItem(ItemData data)
    {
        UseInit(data);
        rangeTiles = CheckRange(data);
        InputManager.Instance.OnMouseMove += CheckEffectRange;
        InputManager.Instance.OnMouseClick += OnClick;
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

        GameManager.Instance.PlayerTransform.curLevel.tiles.TryGetValue(mouseWorldPos, out Tile mouseTile);
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

        Vector2Int mouseWorldPos = Vector2Int.RoundToInt(mouseClickPos);
        _player.curLevel.tiles.TryGetValue(mouseWorldPos, out Tile mouseTile);

        if (rangeTiles.Contains(mouseTile))
        {
            Inventory.Instance.RemoveItem(Inventory.Instance.GetItemIndex(itemData.id));
            FinishUse();
            List<Tile> tiles = new List<Tile>();
            if (itemData.target_range == 0)
            {
                if (itemData.effect_range == 1)
                {
                    tiles = TileUtility.GetNineVisibleTileList(_player.curLevel, mouseTile, false);
                }
                else if (itemData.effect_range >= 2)
                {
                    tiles = TileUtility.GetItemRangedTile(_player.curLevel, mouseTile, itemData.effect_range, false);
                }
            }
            else if (itemData.target_range > 0)
            {
                if (itemData.effect_range == 1)
                {
                    tiles = TileUtility.GetNineVisibleTileList(_player.curLevel, mouseTile, true);
                }
                else
                {
                    tiles = TileUtility.GetItemRangedTile(_player.curLevel, mouseTile, itemData.effect_range, true);
                }
            }

            foreach (Tile tile in tiles)
            {
                if (tile.CharacterStatsOnTile != null)
                {
                    StatusEffectFactory.CreateEffect(itemData.effect_id, itemData.effect_strength, itemData.effect_duration, 10, tile.CharacterStatsOnTile);
                }
            }
            InputManager.Instance.OnMouseClick -= OnClick;

            Destroy(this.gameObject, 0.1f);
        }
        else
        {
            CancelUse();
        }
    }
    public override void CancelUse()
    {
        SubscribeInput();

        InputManager.Instance.OnMouseMove -= CheckEffectRange;
        InputManager.Instance.OnMouseClick -= OnClick;

        // 화면에 표시된 사거리 지우기
        ItemManager.Instance.DestroyRange();
        ItemManager.Instance.DestroyItemRange();

        // 이 게임 오브젝트 파괴
        Destroy(this.gameObject);
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
                dungeonBehavior.CancelItemUse();
            }
        }
    }
}
