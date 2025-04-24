using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    public Dictionary<string, Queue<T>> poolDictionary = new Dictionary<string, Queue<T>>();
    public List<GameObject> prefabs;
    public int initialSize = 5;

    protected virtual void Awake()
    {
        InitObjectPool();
    }
    protected virtual void Start()
    {

    }

    protected void InitObjectPool()
    {

        foreach (GameObject prefab in prefabs)
        {
            if (!poolDictionary.ContainsKey(prefab.name))
            {
                poolDictionary[prefab.name] = new Queue<T>();
            }

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.name = prefab.name;
                obj.transform.SetParent(this.transform);
                obj.gameObject.SetActive(false);
                poolDictionary[prefab.name].Enqueue(obj.GetComponent<T>());
            }
        }
    }

    public virtual T GetFromPool(GameObject prefab,Vector2Int gridPosition , Transform newParent = null)
    {
        T obj;
        if (poolDictionary.ContainsKey(prefab.name) && poolDictionary[prefab.name].Count > 0)
        {
            obj = poolDictionary[prefab.name].Dequeue();
        }
        else // 풀에 남은 오브젝트가 없으면 새로 생성
        {
            obj = Instantiate(prefab).GetComponent<T>();
            obj.name = prefab.name;
        }

        if (newParent != null) obj.transform.SetParent(newParent);
        Vector3 spawnPosition = new Vector3(gridPosition.x, gridPosition.y, 0);
        obj.transform.position = spawnPosition;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public virtual T GetFromPool(GameObject prefab,  Transform spawnTransform, Transform newParent = null)
    {
        T obj;
        if (poolDictionary.ContainsKey(prefab.name) && poolDictionary[prefab.name].Count > 0)
        {
            obj = poolDictionary[prefab.name].Dequeue();
        }
        else // 풀에 남은 오브젝트가 없으면 새로 생성
        {
            obj = Instantiate(prefab).GetComponent<T>();
            obj.name = prefab.name;
        }

        if (newParent != null) obj.transform.SetParent(newParent);
        Vector3 spawnPosition = spawnTransform.position;
        obj.transform.position = spawnPosition;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public virtual T GetFromPool(string prefab, Vector2Int gridPosition,  Transform newParent = null)
    {
        T obj;
        if (poolDictionary.ContainsKey(prefab) && poolDictionary[prefab].Count > 0)
        {
            obj = poolDictionary[prefab].Dequeue();
        }
        else // 풀에 남은 오브젝트가 없으면 새로 생성
        {
            obj = Instantiate(prefabs.Find(x => x.name == prefab)).GetComponent<T>();
            obj.name = prefab;
        }

        if (newParent != null) obj.transform.SetParent(newParent);
        Vector3 spawnPosition =  new Vector3(gridPosition.x, gridPosition.y, 0);
        obj.transform.position = spawnPosition;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public virtual T GetFromPool(string prefab,  Transform spawnTransform , Transform newParent = null)
    {
        T obj;
        if (poolDictionary.ContainsKey(prefab) && poolDictionary[prefab].Count > 0)
        {
            obj = poolDictionary[prefab].Dequeue();
        }
        else // 풀에 남은 오브젝트가 없으면 새로 생성
        {
            obj = Instantiate(prefabs.Find(x => x.name == prefab)).GetComponent<T>();
            obj.name = prefab;
        }

        if (newParent != null) obj.transform.SetParent(newParent);
        Vector3 spawnPosition =  spawnTransform.position;
        obj.transform.position = spawnPosition;
        obj.gameObject.SetActive(true);

        return obj;
    }
    public void ReturnToPool(GameObject obj)
    {
        if (!poolDictionary.ContainsKey(obj.name))
        {
            Destroy(obj);
            return;
        }

        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.gameObject.SetActive(false);
        poolDictionary[obj.name].Enqueue(obj.GetComponent<T>());
    }
}
