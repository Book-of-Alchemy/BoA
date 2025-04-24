using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ProjectileItem : MonoBehaviour
{
    public ItemData itemData;
    public SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    public ProjectileMove projectileMove;

    private Tile _destinationObject; // 오브젝트가 있는 타일
    private Tile _choiceTile; // 선택한 타일
    private Vector2Int _playerPos;
    private Vector2Int itemPos;
    private bool _isObject;



    public void Init(ItemData data, Tile choiceTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        projectileMove = GetComponent<ProjectileMove>();
        _choiceTile = choiceTile;
        SetType(data);
        CheckCharacter();
    }
    // 생성시에 플레이어가 선택한 목적지를 받음
    public void SetDestination(Tile des)
    {
        _choiceTile = des;
    }
    private void SetType(ItemData data)
    {
        itemData = data;
        spriteRenderer.sprite = data.Sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerPos = GameManager.Instance.PlayerTransform.CurTile.gridPosition;

    }

    // 두투윈으로 변경
    void Update()
    {
        itemPos = Vector2Int.RoundToInt(transform.position);
        if (_isObject == true)
            transform.position = Vector2.MoveTowards(transform.position, _choiceTile.gridPosition, 1f * Time.deltaTime);
        if (CheckDistance(itemPos, _choiceTile.gridPosition) < 0.01f)
        {
            List<Tile> tiles;
            if (itemData.target_range == 0 && itemData.effect_type == Effect_Type.Damage)
                tiles = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, _choiceTile, itemData.effect_range, false);
            else
                tiles = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, _choiceTile, itemData.effect_range, true);
            foreach (Tile tile in tiles)
            {
                if (tile.CharacterStatsOnTile != null)
                {
                    GameManager.Instance.PlayerTransform.Attack(tile.CharacterStatsOnTile);
                }
            }
            Debug.Log(itemData.name_en);
            //_projectilItem.item.UseItem(_projectilItem.itemData);
            Destroy(this.gameObject);
        }
    }


    /// <summary>
    /// 선택한 목적지에 적이있는지 확인 로직
    /// </summary>
    public void CheckCharacter()
    {
        if (CheckDistance(_playerPos, _choiceTile.gridPosition) > itemData.target_range)
        {
            return;
        }
        else if (CheckDistance(_playerPos, _choiceTile.gridPosition) <= itemData.target_range)
        {
            CheckObject();
            if (_destinationObject == null)
            {
                Debug.Log("선택한 라인에 적이 없습니다.");
                return;
            }
        }
        transform.position = new Vector3(_playerPos.x, _playerPos.y, 0);
        Debug.Log($"{transform.position}");
    }



    private float CheckDistance(Vector2Int curPos, Vector2Int desPos)
    {
        return Vector2.Distance(curPos, desPos);
    }
    public void CheckObject()
    {
        List<Tile> checkList = TileUtility.GetLineTile(GameManager.Instance.PlayerTransform.curLevel, GameManager.Instance.PlayerTransform.CurTile, _choiceTile, true);
        foreach (Tile tile in checkList)
        {
            if (tile.CharacterStatsOnTile != null)
            {
                _destinationObject = tile;
                _isObject = true;
                break;
            }
        }
    }

    //private void SetEffectType(Effect_Type type)
    //{
    //    switch (type)
    //    {
    //        case Effect_Type.Damage:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<DamageItem>(); break;
    //        case Effect_Type.Heal:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<HealItem>(); break;
    //        case Effect_Type.Buff:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<BuffItem>(); break;
    //        case Effect_Type.Debuff:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<DeBuffItem>(); break;
    //        case Effect_Type.Move:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<MoveItem>(); break;
    //    }
    //}

    // TragetRange가 0인것은 플레이어로부터 시작되는 공격
    // 공격시 아이템을 던질 위치를 받고 그 범위에 해당하는 몬스터 리스트를 받아옴
    // 받아온 리스트를 UseItem에 넣고 로직 처리
    // TragetRange가 0인것은 플레이어를 제외한 나머지 범위의 몬스터리스트를 받아와야한다.

}
