using System.Collections.Generic;
using UnityEngine;

public class UIResourceManager : Singleton<UIResourceManager>
{
    public Dictionary<string, UIBase> uiList = new Dictionary<string, UIBase>();
    //public Dictionary<string, Object> objList = new Dictionary<string, Object>();

    public T LoadUIToKey<T>(string key) where T : UIBase
    {
        if (uiList.TryGetValue(key, out var obj))
        {
            return (T)obj;
        }
        else
        {
            var asset = Resources.Load<T>(key);
            if (asset == null)
            {
                return null;
            }
            uiList[key] = asset;
            return asset;
        }
    }

    public T InstantiateUI<T>(string key, Transform parent = null) where T : UIBase
    {
        var prefab = LoadUIToKey<T>(key);
        if (prefab == null) return null;

        var asset = Instantiate(LoadUIToKey<T>(key), parent);
        asset.name = typeof(T).ToString();
        return asset;
    }

    //public T LoadAsset<T>() where T : MonoBehaviour
    //{
    //    var key = typeof(T).ToString();
    //    if (objList.TryGetValue(key, out var mono))
    //    {
    //        return (T)mono;
    //    }
    //    else
    //    {
    //        var asset = Resources.Load<T>(key);
    //        if (asset != null)
    //            objList.Add(key, asset);
    //        return asset;
    //    }
    //}

    //public T InstantiateAsset<T>(Transform parent = null) where T : MonoBehaviour
    //{
    //    var asset = Instantiate(LoadAsset<T>(), parent);
    //    return asset;
    //}
}
