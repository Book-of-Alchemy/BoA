using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class ResourceManager : Singleton<ResourceManager>
{
    private ItemData[] itemDatas;
    private RecipeData[] recipeDatas;
    public Dictionary<string, ItemData> dicItemData = new Dictionary<string, ItemData>();
    public Dictionary<string, RecipeData> dicRecipeData = new Dictionary<string, RecipeData>();


    private const string itemPath = "Item";
    private const string recipePath = "Recipe";
    void Awake()
    {
        GetResources();
    }

    void GetResources()
    {
        itemDatas = Resources.LoadAll<ItemData>($"{itemPath}");
        if (itemDatas == null)
        {
            Debug.Log("아이템 리소스를 찾지 못했습니다.");
            return;
        }
        for (int i = 0; i < itemDatas.Length; i++)
        {
            dicItemData.Add(itemDatas[i].item_id, itemDatas[i]);
        }
        recipeDatas = Resources.LoadAll<RecipeData>($"{recipePath}");
        if (itemDatas == null)
        {
            Debug.Log("레시피 리소스를 찾지 못했습니다.");
            return;
        }
        for (int i = 0; i < recipeDatas.Length; i++)
        {
            dicRecipeData.Add(recipeDatas[i].recipe_id, recipeDatas[i]);
        }
    }


}
