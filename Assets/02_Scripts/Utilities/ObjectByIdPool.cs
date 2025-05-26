using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ObjectByIdPool<T, U> : MonoBehaviour where T : MonoBehaviour,IPoolableId where U : PrefabWithId
{

    public Dictionary<int, Queue<T>> poolDictionary = new Dictionary<int, Queue<T>>();
    public Dictionary<int, GameObject> prefabLookup = new Dictionary<int, GameObject>();
    public Dictionary<int, U> dataById = new Dictionary<int, U>();
    public List<U> prefabWithIds;

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
        foreach (var prefabInfo in prefabWithIds)
        {
            if (prefabInfo == null || prefabInfo.prefab == null)
            {
                continue;
            }

            int id = prefabInfo.id;
            GameObject prefab = prefabInfo.prefab;

            if (!poolDictionary.ContainsKey(id))
            {
                poolDictionary[id] = new Queue<T>();
                dataById[id] = prefabInfo;
                prefabLookup[id] = prefab;
            }

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                T component = obj.GetComponent<T>();
                component.Id = id;
                poolDictionary[id].Enqueue(component);
            }
        }
    }


    public T GetFromPool(int id, Vector2Int spawnPosition, Transform newParent = null)
    {
        T obj;
        if (poolDictionary.ContainsKey(id) && poolDictionary[id].Count > 0)
        {
            obj = poolDictionary[id].Dequeue();
        }
        else if (prefabLookup.ContainsKey(id))
        {
            GameObject prefab = prefabLookup[id];
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(false);
            obj = newObj.GetComponent<T>();
            obj.Id = id;
        }
        else
        {
            return null;
        }

        if (newParent != null)
            obj.transform.SetParent(newParent);
        else
            obj.transform.SetParent(transform);

        obj.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
        obj.gameObject.SetActive(true);

        return obj;
    }

    public T GetFromPool(int id, Vector2Int spawnPosition,Vector3 offset ,Transform newParent = null)
    {
        T obj;
        if (poolDictionary.ContainsKey(id) && poolDictionary[id].Count > 0)
        {
            obj = poolDictionary[id].Dequeue();
        }
        else if (prefabLookup.ContainsKey(id))
        {
            GameObject prefab = prefabLookup[id];
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(false);
            obj = newObj.GetComponent<T>();
        }
        else
        {
            return null;
        }

        if (newParent != null)
            obj.transform.SetParent(newParent);
        else
            obj.transform.SetParent(transform);

        obj.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0) + offset;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void ReturnToPool(T obj)
    {
        if (!poolDictionary.ContainsKey(obj.Id))
        {
            Destroy(obj.gameObject);
            return;
        }

        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.gameObject.SetActive(false);
        poolDictionary[obj.Id].Enqueue(obj);
    }


}
