using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DamageItem : BaseItem
{
    [Header("사용 아이템 정보")]
    private List<Tile> rangeTiles = new List<Tile>();
    private Tile mouseClickTile;
    private int targetRange;
    public float moveSpeed = 5f;


    /// <summary>
    /// UseItem 위치, 정보, 스프라이트 초기화
    /// 현재 init은 dropitem에서도 사용중임 useitem에서 생성되는것은 다른 메서드로 해야함
    /// drop시 위치가 맞지않는 현상
    /// </summary>
    /// <param name="data"></param>
    public void UseInit(ItemData data)
    {
        _player = GameManager.Instance.PlayerTransform;
        itemData = data;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(_player.CurTile.gridPosition.x, _player.CurTile.gridPosition.y + 0.5f, 0);
        transform.localScale = new Vector3(2, 2, 1);
        SetType(data);
    }

    /// <summary>
    /// 아이템 사용 매서드
    /// </summary>
    /// <param name="data"></param>
    public override void UseItem(ItemData data)
    {
        UseInit(data);
        dungeonBehavior.DuplicationlItemUse();
        rangeTiles = CheckRange(data);
        InputManager.Instance.OnMouseMove += CheckEffectRange;
        InputManager.Instance.OnMouseClick += OnClick;
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
    /// <summary>
    /// 아이템 사용 후 사용하려는 위치에 마우스를 눌렀을때 동작하는 매서드
    /// </summary>
    /// <param name="mouseClickPos"></param>
    public void OnClick(Vector3 mouseClickPos)
    {
        InputManager.Instance.OnMouseMove -= CheckEffectRange; // 마우스 위치에따라 보여주는 매서드 구독해제
        ItemManager.Instance.DestroyItemRange(); // 아이템 사거리 삭제
        ItemManager.Instance.DestroyRange(); // 아이템 효과 범위 삭제
        InputManager.Instance.OnMouseClick -= OnClick;
        

        Vector2Int mouseWorldPos = Vector2Int.RoundToInt(mouseClickPos);
        GameManager.Instance.PlayerTransform.curLevel.tiles.TryGetValue(mouseWorldPos, out mouseClickTile);

        // 사거리 내에 타일을 클릭했는지 확인하는 조건, 사거리 내의 타일을 클릭했다면 효과범위내에 대상들이 있는지 확인
        if (rangeTiles.Contains(mouseClickTile))
        {
            Inventory.Instance.RemoveItem(Inventory.Instance.GetItemIndex(itemData.id));
            List<Tile> tiles = TileUtility.GetLineTile(_player.curLevel, _player.CurTile, mouseClickTile);
            Tile targetTile = mouseClickTile;
            foreach (Tile objectTile in tiles)
            {
                if (objectTile.CharacterStatsOnTile != null)
                {
                    targetTile = objectTile;
                    break;
                }
            }

            SoundManager.Instance.Play(DamageCalculator.GetIntroSoundID(itemData.tags));
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
                        AttackBehavior(itemData, targetTile);
                }
                ));
        }
        else
        {
            CancelUse();
            //SubscribeInput();
        }
    }



    /// <summary>
    /// 공격 로직
    /// </summary>
    /// <param name="data"></param>
    private void AttackBehavior(ItemData data, Tile targetTile)
    {
        int damageValue = data.effect_value;
        int monsterCount;
        List<Tile> tiles = new List<Tile>();
        if (targetRange == 0)
        {
            if (data.effect_range == 1)
            {
                tiles = TileUtility.GetNineVisibleTileList(_player.curLevel, targetTile, false);
            }
            else if (data.effect_range >= 2)
            {
                tiles = TileUtility.GetItemRangedTile(_player.curLevel, targetTile, data.effect_range, false);
            }
        }
        else if (targetRange > 0)
        {
            if (data.effect_range == 1)
            {
                tiles = TileUtility.GetNineVisibleTileList(_player.curLevel, targetTile, true);
            }
            else
            {
                tiles = TileUtility.GetItemRangedTile(_player.curLevel, targetTile, data.effect_range, true);
            }
        }
        // 마법 정밀 조준 아티팩트 적용부분
        monsterCount = tiles.Count(x => x.CharacterStatsOnTile != null);

        if (monsterCount >= 3 && data.tags.Contains(Tag.Scroll) && _player.isPrecisionAim)
        {
            _player.statBlock.AddModifier(StatType.FinalDmg, new StatModifier("PrecisionAimforMagic", 70, ModifierType.Precent));
        }

        foreach (Tile ojTile in tiles)
        {
            switch (data.attribute)
            {
                case Attribute.Fire:
                    EffectProjectileManager.Instance.PlayEffect(ojTile.gridPosition, 30010);
                    break;
                case Attribute.Earth:
                    EffectProjectileManager.Instance.PlayEffect(ojTile.gridPosition, 30012);
                    break;
                case Attribute.Dark:
                    EffectProjectileManager.Instance.PlayEffect(ojTile.gridPosition, 30021);
                    break;
            }
            if (ojTile.CharacterStatsOnTile != null)
            {
                if (data.attribute == Attribute.None)
                    EffectProjectileManager.Instance.PlayEffect(ojTile.gridPosition, 30013);
                if (data.tags.Contains(Tag.Scroll) && _player.isManaOverload) // 마나오버로드 아티팩트 확인하는 부분
                {
                    if (data.mp_cost < _player.CurrentMana)
                    {
                        damageValue = data.effect_value + data.effect_value;
                        _player.ChangeMana(-data.mp_cost);
                    }
                }
                ojTile.CharacterStatsOnTile.TakeDamage(new DamageInfo(damageValue, GetDamageType(data.attribute), _player, ojTile.CharacterStatsOnTile, false, data.tags, data.effect_id));
            }
        }

        TileRuleProccessor.ProcessTileReactions(new DamageInfo(damageValue, GetDamageType(data.attribute), _player, null, false, data.tags, data.effect_id), tiles);

        if (monsterCount >= 3 && data.tags.Contains(Tag.Scroll) && _player.isPrecisionAim)
        {
            _player.statBlock.RemoveModifier(StatType.FinalDmg, "PrecisionAimforMagic");
        }

        SoundManager.Instance.Play(DamageCalculator.GetImpactSoundID(data.attribute));
        FinishUse();
        Destroy(this.gameObject, 0.1f);
    }

    private DamageType GetDamageType(Attribute attribute)
    {
        return attribute switch
        {
            Attribute.None => DamageType.None,
            Attribute.Fire => DamageType.Fire,
            Attribute.Water => DamageType.Water,
            Attribute.Cold => DamageType.Cold,
            Attribute.Lightning => DamageType.Lightning,
            Attribute.Earth => DamageType.Earth,
            Attribute.Wind => DamageType.Wind,
            Attribute.Light => DamageType.Light,
            Attribute.Dark => DamageType.Dark,
            _ => DamageType.None,
        };
    }

    /// <summary>
    /// 아이템 사용시 아이템 효과 범위를 마우스 움직임에 따라 실시간으로 보여주는 매서드
    /// </summary>
    /// <param name="mousePos"></param>
    public void CheckEffectRange(Vector3 mousePos)
    {
        ItemManager.Instance.DestroyItemRange();

        Vector2Int mouseWorldPos = Vector2Int.RoundToInt(mousePos);

        GameManager.Instance.PlayerTransform.curLevel.tiles.TryGetValue(mouseWorldPos, out Tile mouseTile);
        if (rangeTiles.Contains(mouseTile))
        {
            List<Tile> checkItemRangeTiles = new List<Tile>();

            if (targetRange == 0)
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

    /// <summary>
    /// 아이템 사용시 클릭 가능한 아이템 사거리를 표시해주고 타일리스트를 반환해주는 매서드
    /// </summary>
    public List<Tile> CheckRange(ItemData data)
    {
        ItemManager.Instance.DestroyRange();


        if (_player.isMarksman && data.tags.Contains(Tag.Throw))
        {
            targetRange = data.target_range + 1;
        }
        else
            targetRange = data.target_range;

        List<Tile> checkRangeTiles = new List<Tile>();
        if (targetRange == 0)
            checkRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, _player.CurTile, targetRange, true);
        else if (targetRange == 1)
            checkRangeTiles = TileUtility.GetNineVisibleTileList(_player.curLevel, _player.CurTile, true);
        else if (targetRange >= 2)
            checkRangeTiles = TileUtility.GetItemRangedTile(_player.curLevel, _player.CurTile, targetRange, true);

        ItemManager.Instance.CreateRange(checkRangeTiles);

        return checkRangeTiles;
    }


    public void SubscribeInput()
    {
        if (_player != null)
        {
            if (dungeonBehavior != null)
            {
                dungeonBehavior.SSubscribeInput();
                dungeonBehavior.CancelItemUse();
            }
        }
    }
}
