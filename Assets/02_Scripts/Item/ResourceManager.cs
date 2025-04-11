using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class ResourceManager : Singleton<ResourceManager>
{
    private ItemData[] itemDatas;
    public RecipeData[] recipeDatas;
    public Dictionary<int, ItemData> dicItemData = new Dictionary<int, ItemData>();
    public Dictionary<Effect_Type,BaseItem> effectTypeData = new Dictionary<Effect_Type, BaseItem>();
    private int intNullvalue = -1;


    private const string itemPath = "Item";
    private const string recipePath = "Recipe";
    protected override void Awake()
    {
        base.Awake();
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
            if (itemDatas[i].id != intNullvalue)
            dicItemData.Add(itemDatas[i].id, itemDatas[i]);
        }
        recipeDatas = Resources.LoadAll<RecipeData>($"{recipePath}");
        if (itemDatas == null)
        {
            Debug.Log("레시피 리소스를 찾지 못했습니다.");
            return;
        }
        EffectTypeAdd();
        //for (int i = 0; i < recipeDatas.Length; i++)
        //{
        //    dicRecipeData.Add(recipeDatas[i].recipe_id, recipeDatas[i]);
        //}
    }

    void EffectTypeAdd()
    {
        effectTypeData.Add(Effect_Type.Damage, new DamageItem());
    }


}
