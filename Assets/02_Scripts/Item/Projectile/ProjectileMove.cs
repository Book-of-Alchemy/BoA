using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class ProjectileMove : MonoBehaviour
{
    private ProjectileItem _projectilItem;
    private Tile _destinationObject; // 오브젝트가 있는 타일
    private Tile _choiceTile; // 선택한 타일
    private Vector2Int _playerPos;
    private Vector2Int itemPos;
    private bool _isObject;

    // Start is called before the first frame update
    void Start()
    {
        _projectilItem = GetComponent<ProjectileItem>();
        _playerPos = GameManager.Instance.PlayerTransform.curTile.gridPosition;

    }

    // 두투윈으로 변경
    void Update()
    {
        itemPos = Vector2Int.RoundToInt(transform.position);
        if (_isObject == true)
            transform.position = Vector2.MoveTowards(transform.position, _choiceTile.gridPosition, 1f * Time.deltaTime);
        if (CheckDistance(itemPos, _choiceTile.gridPosition) < 0.01f)
        {
            _projectilItem.item.UseItem(_projectilItem.itemData);
            Destroy(this.gameObject);
        }
    }


    /// <summary>
    /// 선택한 목적지에 적이있는지 확인 로직
    /// </summary>
    public void Init()
    {
        if (CheckDistance(_playerPos, _choiceTile.gridPosition) > _projectilItem.itemData.target_range)
        {
            return;
        }
        else if (CheckDistance(_playerPos, _choiceTile.gridPosition) <= _projectilItem.itemData.target_range)
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

    // 생성시에 플레이어가 선택한 목적지를 받음
    public void SetDestination(Tile des)
    {
        _choiceTile = des;
    }

    private float CheckDistance(Vector2Int curPos, Vector2Int desPos)
    {
        return Vector2.Distance(curPos, desPos);
    }
    public void CheckObject()
    {
        List<Tile> checkList = TileUtility.GetLineTile(GameManager.Instance.PlayerTransform.curLevel, GameManager.Instance.PlayerTransform.curTile, _choiceTile, true);
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


}
