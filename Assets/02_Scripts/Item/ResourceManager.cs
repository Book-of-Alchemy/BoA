
using System.Collections.Generic;
using UnityEngine;


public class ResourceManager : Singleton<ResourceManager>
{
    private ItemData[] itemDatas;
    public RecipeData[] recipeDatas;
    public GameObject typeObjectPrefab;
    //public GameObject typeObject;
    public Dictionary<int, ItemData> dicItemData = new Dictionary<int, ItemData>();
    //public Dictionary<Effect_Type,BaseItem> effectTypeData = new Dictionary<Effect_Type, BaseItem>();
    private int intNullvalue = -1;


    private const string itemPath = "Items";
    private const string recipePath = "Recipes";
    private const string TypePath = "TypeObject";
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
            return;
        }
        typeObjectPrefab = Resources.Load<GameObject>($"{TypePath}/TypeObject");
        //typeObject = Instantiate(_typeObjectPrefab);
        //typeObject.transform.SetParent(this.transform);

        //EffectTypeAdd();
        //for (int i = 0; i < recipeDatas.Length; i++)
        //{
        //    dicRecipeData.Add(recipeDatas[i].recipe_id, recipeDatas[i]);
        //}
    }

    //void EffectTypeAdd()
    //{
    //    DamageItem damage = typeObject.GetComponent<DamageItem>();
    //    BuffItem buff = typeObject.GetComponent<BuffItem>();
    //    DeBuffItem debuff = typeObject.GetComponent<DeBuffItem>();
    //    HealItem heal = typeObject.GetComponent <HealItem>();
    //    MoveItem move = typeObject.GetComponent<MoveItem>();
    //
    //
    //    effectTypeData.Add(Effect_Type.Damage, damage);
    //    effectTypeData.Add(Effect_Type.Buff, buff);
    //    effectTypeData.Add(Effect_Type.Debuff, debuff);
    //    effectTypeData.Add(Effect_Type.Heal, heal);
    //    effectTypeData.Add(Effect_Type.Move, move);
    //}


}
