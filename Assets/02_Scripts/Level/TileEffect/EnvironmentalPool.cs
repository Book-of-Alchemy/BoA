using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalPool : MonoBehaviour
{
    /// <summary>
    /// 환경 프리팹을 가져오고 되돌리는 역할만함
    /// </summary>
    public Queue<EnvironmentPrefab> environmentPrefabs = new Queue<EnvironmentPrefab>();
    
    public GameObject prefab;
    public int initialSize = 20;

    private void Awake()
    {
        InitObjectPool();
    }

    private void InitObjectPool()
    {
        EnvironmentalFactory.Instance.environmentalPool = this;
        transform.SetParent(EnvironmentalFactory.Instance.transform);
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(this.transform);
            obj.gameObject.SetActive(false);
            environmentPrefabs.Enqueue(obj.GetComponent<EnvironmentPrefab>());
        }
    }

    public EnvironmentPrefab GetFromPool(Tile tile, Transform newParent = null)
    {
        EnvironmentPrefab obj;
        if (environmentPrefabs.Count > 0)
        {
            obj = environmentPrefabs.Dequeue();
            TileEffect tileEffect = obj.GetComponent<TileEffect>();
            if(tileEffect == null)
            {
                Debug.Log("getfrompool check");
            }
            DestroyImmediate(tileEffect);
            //Destroy(tileEffect);
        }
        else // 풀에 남은 오브젝트가 없으면 새로 생성
        {
            obj = Instantiate(prefab).GetComponent<EnvironmentPrefab>();
        }

        if (newParent != null) obj.transform.SetParent(newParent);
        Vector2Int gridPosition = tile.gridPosition;
        Vector3 spawnPosition = new Vector3(gridPosition.x, gridPosition.y, 0);
        obj.transform.position = spawnPosition;
        obj.CurTile = tile;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void ReturnToPool(EnvironmentPrefab obj)
    {
        obj.OnReturn();
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.gameObject.SetActive(false);
        environmentPrefabs.Enqueue(obj);
    }
}
