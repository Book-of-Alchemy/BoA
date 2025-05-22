using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Alchemy : MonoBehaviour
{
    public RecipeData resultRecipe;
    public Dictionary<string, RecipeData> recipeKey = new Dictionary<string, RecipeData>();
    private int intNullValue = -1;
    private bool isReady = false;

    private void Start()
    {
        Init();
        //CreateItem(ResourceManager.Instance.dicItemData[201001], 4, ResourceManager.Instance.dicItemData[201002], 3);
    }

    private void Init()
    {
        CreateRecipeKey();
    }
    private void CreateRecipeKey()
    {
        foreach (var recipe in ResourceManager.Instance.recipeDatas)
        {
            string Key = ChangeKey(recipe);
            recipeKey[Key] = recipe;
        }
    }

    private string ChangeKey(RecipeData data)
    {
        var recipeMaterials = new List<int>();
        if (data.material_1_item_id != intNullValue) recipeMaterials.Add(data.material_1_item_id);
        if (data.material_2_item_id != intNullValue) recipeMaterials.Add(data.material_2_item_id);
        if (data.material_3_item_id != intNullValue) recipeMaterials.Add(data.material_3_item_id);
        recipeMaterials.Sort();
        return string.Join("", recipeMaterials);
    }
    private string GetMaterialsKey(List<(ItemData materials, int amount)> materials)
    {
        var curMaterials = new List<int?>();
        foreach (var material in materials)
        {
            if(material.materials!= null)
            {
                curMaterials.Add(material.materials.id);
            }
        }
        curMaterials.Sort();
        return string.Join("", curMaterials);
    }
    public (bool, RecipeData, int) CreateItem(ItemData material1, int material1Amount, ItemData material2, int material2Amount, ItemData material3 = null, int material3Amount = 0)
    {
        Debug.Log($"재료 1 과 수량 :{material1},{material1Amount}");
        Debug.Log($"재료 2 와 수량 :{material2},{material2Amount}");
        if(material3!=null)
            Debug.Log($"재료 3 과 수량 :{material3},{material3Amount}");
        List<(ItemData materials, int amount)> materials = new List<(ItemData materials, int amount)>();
        materials.Add((material1, material1Amount));
        materials.Add((material2, material2Amount));
        materials.Add((material3, material3Amount));
        string curMaterialKey = GetMaterialsKey(materials);
        if (recipeKey.ContainsKey(curMaterialKey))
        {
            resultRecipe = recipeKey[curMaterialKey];
            // 레시피 존재한다면 수량 확인 후 제작
            CheckMaterial(materials[0], resultRecipe);
            if (isReady == false) return (isReady, null,0);
            CheckMaterial(materials[1], resultRecipe);
            if (isReady == false) return (isReady, null, 0);
            CheckMaterial(materials[2], resultRecipe);
            if (isReady == false) return (isReady, null, 0);
        }
        else 
        {
            Debug.Log("레시피가 없습니다.");
            return (isReady, null, 0);
        }

        if (isReady)
        {
            Debug.Log($"제작 성공 {resultRecipe.recipe_name_kr} : {resultRecipe.output_amount} ");
            //ItemData item = ResourceManager.Instance.dicItemData[resultRecipe.output_item_id];
            //return (isReady, item, amount); 
            
            //반환용 레시피와 수량
            int amount = resultRecipe.output_amount;
            RecipeData recipe = resultRecipe;
            GameManager.Instance.PlayerTransform.ChangeMana(-recipe.mp_cost);

            return (isReady, recipe, amount); 
        }
        else
        {
            Debug.Log("제작 실패");
            return (isReady, null, 0);
        }
        // 레시피 결과물 리턴 추가

    }

    private void CheckMaterial((ItemData materials, int amount) materials, RecipeData recipe)
    {
        // 재료의 수량과, 아이디가 null이면 리턴
        if (materials.materials == null || materials.amount == 0)
        {
            Debug.Log("재료가 없습니다.");
            return;
        }
        else if (materials.materials.id == recipe.material_1_item_id)
        {
            // 수량 체크
            CheckAmount(materials, recipe.material_1_amount);
        }
        else if (materials.materials.id == recipe.material_2_item_id)
        {
            CheckAmount(materials, recipe.material_2_amount);
        }
        else if (materials.materials.id == recipe.material_3_item_id)
        {
            CheckAmount(materials, recipe.material_3_amount);
        }
    }

    private void CheckAmount((ItemData materials, int amount) materials, int recipeAmount)
    {
        if (recipeAmount == 0) return;
        //수량확인
        if (materials.amount < recipeAmount)
        {
            isReady = false;
            Debug.Log($"{materials.materials.name_kr}의 수량이 부족합니다.");
        }
        else if (materials.amount >= recipeAmount)
        {
            isReady = true;
        }
    }

    public List<int> GetCraftableIds(HashSet<int> craftItemIds)
    {
        HashSet<int> requiredItemIds = new HashSet<int>(); // 인자를 recipeItemIds와 대조하여 제외하고 리턴할 변수
        HashSet<int> recipeItemIds = new HashSet<int>();

        foreach (var recipe in ResourceManager.Instance.recipeDatas)
        {
            recipeItemIds.Clear();

            //레시피 재료 아이템 1,2,3번의 비어있는지 검사
            recipeItemIds.Add(recipe.material_1_item_id);
            recipeItemIds.Add(recipe.material_2_item_id);
            if (recipe.material_3_item_id != -1) 
                recipeItemIds.Add(recipe.material_3_item_id);

            //현재 선택한 재료들이 recipeDatas에 배열에서 인덱스 recipe에 부분집합인지 검사
            if (craftItemIds.IsSubsetOf(recipeItemIds))
            {
                //recipeMaterials에서 부분집합인 부분을 제외
                recipeItemIds.ExceptWith(craftItemIds);
                // 남은 재료가 있다면 추가
                requiredItemIds.UnionWith(recipeItemIds);
            }
        }
        return requiredItemIds.ToList();
    }
    public InventoryItem GetCraftResultPreview(List<int> curCraftTableIds)
    {
        // 재료 개수 검사
        if (curCraftTableIds == null || (curCraftTableIds.Count != 2 && curCraftTableIds.Count != 3))
        {
            Debug.LogWarning("재료가 없거나 일치하지 않음.");
            return null;
        }

        curCraftTableIds.Sort();

        var recipeMaterial_Id = new List<int>();

        //레시피 데이터를 반복하며 검사
        foreach (var recipe in ResourceManager.Instance.recipeDatas)
        {
            recipeMaterial_Id.Clear();

            if (recipe.material_1_item_id != intNullValue) recipeMaterial_Id.Add(recipe.material_1_item_id);
            if (recipe.material_2_item_id != intNullValue) recipeMaterial_Id.Add(recipe.material_2_item_id);
            if (recipe.material_3_item_id != intNullValue) recipeMaterial_Id.Add(recipe.material_3_item_id);

            //제작필요 갯수를 검사(2개라면 바로 다음 반복)
            if (recipeMaterial_Id.Count != curCraftTableIds.Count)
                continue;

            //curCraftTableIds와 인덱스를 맞추기 위한 정렬
            recipeMaterial_Id.Sort();

            bool isMatch = true;
            for (int i = 0; i < curCraftTableIds.Count; i++)
            {
                //일치하지 않으면 즉시 반복취소
                if (curCraftTableIds[i] != recipeMaterial_Id[i])
                {
                    isMatch = false;
                    break;
                }
            }

            //전부 맞았다면 
            if (isMatch)
            {
                // 일치하는 레시피를 찾았을 경우
                if (ResourceManager.Instance.dicItemData.TryGetValue(recipe.output_item_id, out var itemData))
                {
                    InventoryItem resultItem = new InventoryItem();
                    resultItem.AddItem(itemData,recipe.output_amount);
                    return resultItem;
                }
            }
        }

        Debug.Log("일치하는 레시피가 없음");
        return null;
    }

    //private void CreateItem(ItemData material1, int material1Amount, ItemData material2, int material2Amount, ItemData material3 = null, int material3Amount = 0)
    //{
    //    if (material3 == null)
    //    {
    //        foreach (var recipes in ResourceManager.Instance.recipeDatas)
    //        {
    //            if ((recipes.material_1_item_id == material1.item_id) || (recipes.material_1_item_id == material2.item_id))
    //            {
    //                firstExceptionRecipes.Add(recipes);
    //            }
    //        }
    //
    //        foreach (var recipes in firstExceptionRecipes)
    //        {
    //            if ((recipes.material_2_item_id == material1.item_id) || (recipes.material_2_item_id == material2.item_id))
    //            {
    //                secondExceptionRecipes.Add(recipes);
    //            }
    //        }
    //
    //        foreach (var recipes in secondExceptionRecipes)
    //        {
    //            if ((recipes.material_3_item_id == null))
    //            {
    //                thirdExceptionRecipes.Add(recipes);
    //            }
    //        }
    //    }
    //    else if (material3 != null)
    //    {
    //        foreach (var recipes in ResourceManager.Instance.recipeDatas)
    //        {
    //            if ((recipes.material_1_item_id == material1.item_id) || (recipes.material_1_item_id == material2.item_id) || (recipes.material_1_item_id == material3.item_id))
    //            {
    //                firstExceptionRecipes.Add(recipes);
    //            }
    //        }
    //
    //        foreach (var recipes in firstExceptionRecipes)
    //        {
    //            if ((recipes.material_2_item_id == material1.item_id) || (recipes.material_2_item_id == material2.item_id) || (recipes.material_2_item_id == material3.item_id))
    //            {
    //                secondExceptionRecipes.Add(recipes);
    //            }
    //        }
    //        foreach (var recipes in secondExceptionRecipes)
    //        {
    //            if ((recipes.material_3_item_id == material1.item_id) || (recipes.material_3_item_id == material2.item_id) || (recipes.material_3_item_id == material3.item_id))
    //            {
    //                thirdExceptionRecipes.Add(recipes);
    //            }
    //        }
    //    }
    //
    //    if (thirdExceptionRecipes.Count == 0)
    //    {
    //        Debug.Log("레시피를 찾을 수 없습니다.");
    //    }
    //    else if (thirdExceptionRecipes.Count > 1)
    //    {
    //        Debug.Log("찾은 레시피의 갯수가 많습니다.");
    //    }
    //    else if (thirdExceptionRecipes.Count == 1)
    //    {
    //        resultRecipe = thirdExceptionRecipes[0];
    //    }
    //
    //    if (material3 == null)
    //    {
    //        if ((resultRecipe.material_1_item_id == material1.item_id)&& (resultRecipe.material_2_item_id == material2.item_id))
    //        {
    //            if (resultRecipe.material_1_count > material1Amount)
    //            {
    //                isMaterial1 = false;
    //                Debug.Log("Material1의 수량이 부족하다.");
    //                //수량이 부족하다
    //            }
    //            if (resultRecipe.material_2_count > material2Amount)
    //            {
    //                isMaterial2 = false;
    //                //수량이 부족하다
    //            }
    //
    //            if (resultRecipe.material_1_count <= material1Amount && resultRecipe.material_2_count < material2Amount)
    //            {
    //                // 수량 충분
    //                isMaterial1 = true;
    //                isMaterial2 = true;
    //            }
    //        }
    //        else if (resultRecipe.material_1_item_id == material2.item_id)
    //        {
    //
    //        }
    //    }
    //
    //    
    //}

}
