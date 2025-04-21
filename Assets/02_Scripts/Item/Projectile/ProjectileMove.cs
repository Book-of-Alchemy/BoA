using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    private ItemController controller;
    private Tile destinationObject; // 오브젝트가 있는 타일
    private List<Tile> _rangedTile = new List<Tile>(); // 오브젝트가 있는타일 기준으로 범위에있는 오브젝트 타일
    private List<CharacterStats> _rangedObject = new List<CharacterStats>();
    private Tile choiceTile; // 선택한 타일
    private Vector2Int _playerPos;
    private Vector2Int itemPos;
    private bool _isObject;

    


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ItemController>();
        _playerPos = GameManager.Instance.PlayerTransform.curTile.gridPosition;


    }

    // Update is called once per frame
    void Update()
    {
        itemPos = Vector2Int.RoundToInt(transform.position);
        if(_isObject == true)
            transform.position = Vector2.MoveTowards(transform.position, choiceTile.gridPosition, 0.1f*Time.deltaTime);
        if (CheckDistance(itemPos, choiceTile.gridPosition) < 0.01f)
        {
            if(controller.data.target_range == 0 && controller.data.effect_type == Effect_Type.Damage)
                _rangedTile = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, destinationObject, controller.data.effect_range, false);
            else
                _rangedTile = TileUtility.GetRangedTile(GameManager.Instance.PlayerTransform.curLevel, destinationObject, controller.data.effect_range,true);
            //아이템 사용
            foreach(Tile tile in _rangedTile)
            {
                if(tile.characterStats !=null)
                    _rangedObject.Add(tile.characterStats);
            }
            if (_rangedObject.Count >= 1)
            {
                controller.Item.UseItem(_rangedObject);
                Destroy(this.gameObject);
            }

            else
            {
                Debug.Log("사용하려는 아이템 주변에 적이 없습니다.");
            }
        }
    }

    public void Init()
    {
        if (CheckDistance(_playerPos, choiceTile.gridPosition) > controller.data.target_range)
        {
            return;
        }
        else if (CheckDistance(_playerPos, choiceTile.gridPosition) <= controller.data.target_range)
        {
            CheckObject();
            if (destinationObject == null)
            {
                Debug.Log("선택한 라인에 적이 없습니다.");
                return;
            }
        }
        transform.position = new Vector3(_playerPos.x,_playerPos.y,0);
        Debug.Log($"{transform.position}");
    }

    // 생성시에 플레이어가 선택한 목적지를 받음
    public void SetDestination(Tile des)
    {
        choiceTile = des;
    }

    private float CheckDistance(Vector2Int curPos, Vector2Int desPos)
    {
        return Vector2.Distance(curPos, desPos);
    }
    public void CheckObject()
    {
        List<Tile> checkList = TileUtility.GetLineTile(GameManager.Instance.PlayerTransform.curLevel, GameManager.Instance.PlayerTransform.curTile, choiceTile,true);
        foreach (Tile tile in checkList)
        {
            if (tile.characterStats != null)
            {
                destinationObject = tile;
                _isObject = true;
                break;
            }
        }
    }


}
