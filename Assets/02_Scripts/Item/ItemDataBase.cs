
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Data Base")]
public class ItemDataBase : ScriptableObject
{
    public List<ItemData> itemDatas;
    public List<RecipeData> recipeDatas;
    public GameObject typeObjectPrefab;
    public Dictionary<int, ItemData> dicItemData = new Dictionary<int, ItemData>();
    public Dictionary<Item_Type,List<ItemData>> dicItemByType = new Dictionary<Item_Type, List<ItemData>>();
    public Dictionary<int, RecipeData> recipeById = new Dictionary<int, RecipeData>();
    private int intNullvalue = -1;

    private void OnValidate()
    {
        ArrangeItemData();
        ArrangeRecipeData();
    }

    private void ArrangeItemData()
    {
        dicItemData.Clear();
        dicItemByType.Clear();
        foreach (var data in itemDatas)
        {
            if (data == null) continue;

            if (!dicItemData.ContainsKey(data.id))
            {
                dicItemData.Add(data.id, data);
            }
            else
            {
            }

            if (!dicItemByType.ContainsKey(data.item_type))
            {
                dicItemByType[data.item_type] = new List<ItemData> { data };
            }
            else
            {
                dicItemByType[data.item_type].Add(data);
            }
        }
    }

    private void ArrangeRecipeData()
    {
        recipeById.Clear();
        foreach (var data in recipeDatas)
        {
            if (data == null) continue;

            if (!recipeById.ContainsKey(data.id))
            {
                recipeById.Add(data.id, data);
            }
            else
            {
            }
        }
    }

    public ItemData GetItemDataById(int id)
    {
        dicItemData.TryGetValue(id, out var data);
        return data;
    }

    public RecipeData GetRecipeDataById(int id)
    {
        recipeById.TryGetValue(id, out var data);
        return data;
    }
}
