using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Alchemy : MonoBehaviour
{
    public RecipeData resultRecipe;
    public Dictionary<string, RecipeData> recipeKey = new Dictionary<string, RecipeData>();
    private bool isReady = false;

    private void Start()
    {
        Init();
        CreateItem(ResourceManager.Instance.dicItemData["I1003"], 4, ResourceManager.Instance.dicItemData["I1005"], 3, ResourceManager.Instance.dicItemData["I1004"],6);
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
        var recipeMaterials = new List<string>();
        if (data.material_1_item_id != null) recipeMaterials.Add(data.material_1_item_id);
        if (data.material_2_item_id != null) recipeMaterials.Add(data.material_2_item_id);
        if (data.material_3_item_id != null) recipeMaterials.Add(data.material_3_item_id);
        recipeMaterials.Sort();
        return string.Join("", recipeMaterials);
    }
    private string GetMaterialsKey(List<(ItemData materials, int amount)> materials)
    {
        var curMaterials = new List<string>();
        foreach (var material in materials)
        {
            if(material.materials!= null)
            {
                curMaterials.Add(material.materials.item_id);
            }
        }
        curMaterials.Sort();
        return string.Join("", curMaterials);
    }
    public bool CreateItem(ItemData material1, int material1Amount, ItemData material2, int material2Amount, ItemData material3 = null, int material3Amount = 0)
    {
        List<(ItemData materials, int amount)> materials = new List<(ItemData materials, int amount)>();
        materials.Add((material1, material1Amount));
        materials.Add((material2, material2Amount));
        materials.Add((material3, material2Amount));
        string curMaterialKey = GetMaterialsKey(materials);
        if (recipeKey.ContainsKey(curMaterialKey))
        {
            resultRecipe = recipeKey[curMaterialKey];
            // 레시피 존재한다면 수량 확인 후 제작
            CheckMaterial(materials[0], resultRecipe);
            CheckMaterial(materials[1], resultRecipe);
            CheckMaterial(materials[2], resultRecipe);
        }
        else 
        {
            Debug.Log("레시피가 없습니다.");
            return false;
        }

        if (isReady)
        {
            Debug.Log($"제작 성공 {resultRecipe.recipe_name_kr} : {resultRecipe.output_amount} ");
            return true; 
        }
        else
        {
            Debug.Log("제작 실패");
            return false;
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
        else if (materials.materials.item_id == recipe.material_1_item_id)
        {
            // 수량 체크
            CheckAmount(materials, recipe.material_1_count);
        }
        else if (materials.materials.item_id == recipe.material_2_item_id)
        {
            CheckAmount(materials, recipe.material_2_count);
        }
        else if (materials.materials.item_id == recipe.material_3_item_id)
        {
            CheckAmount(materials, recipe.material_3_count);
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
