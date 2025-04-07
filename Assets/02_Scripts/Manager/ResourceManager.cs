using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public Dictionary<string, Object> uiList = new Dictionary<string, Object>();
    public Dictionary<string, Object> objList = new Dictionary<string, Object>();

    public T LoadUIToKey<T>(string key) where T : UnityEngine.Object
    {
        if (uiList.TryGetValue(key, out var obj))
        {
            return (T)obj;
        }
        else
        {
            var asset = Resources.Load<T>(key);
            if (asset != null)
                uiList.Add(key, asset);
            return asset;
        }
    }

    public T InstantiateUI<T>(string key, Transform parent = null) where T : Object
    {
        var asset = Instantiate(LoadUIToKey<T>(key), parent);
        return asset;
    }

    public T LoadAsset<T>() where T : MonoBehaviour
    {
        var key = typeof(T).ToString();
        if (objList.TryGetValue(key, out var mono))
        {
            return (T)mono;
        }
        else
        {
            var asset = Resources.Load<T>(key);
            if (asset != null)
                objList.Add(key, asset);
            return asset;
        }
    }

    public T InstantiateAsset<T>(Transform parent = null) where T : MonoBehaviour
    {
        var asset = Instantiate(LoadAsset<T>(), parent);
        return asset;
    }
}
