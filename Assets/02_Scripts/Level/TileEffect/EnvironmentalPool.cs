using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalPool : MonoBehaviour
{
    public Queue<EnvironmentPrefab> environmentPrefabs = new Queue<EnvironmentPrefab>();
    public GameObject prefab;
    public int initialSize = 5;

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
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.OnReturn();
        obj.gameObject.SetActive(false);
        environmentPrefabs.Enqueue(obj.GetComponent<EnvironmentPrefab>());
    }
}
