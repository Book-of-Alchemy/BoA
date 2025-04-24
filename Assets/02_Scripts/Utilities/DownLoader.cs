using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using Newtonsoft.Json;




public class DownLoader : EditorWindow
{
    string itemDBUrl = "https://opensheet.elk.sh/1kt6Cg9_sC3G4imRU9_b0_o0j-XP5KUCrDXyvX__BdH4/Item_DB";
    string itemJsonSavePath = "Assets/Resources/Json/ItemData.json";
    string recipeDBUrl = "https://opensheet.elk.sh/1CPbtP4IkSU3za-8Y-BjNnG6_fGdG-z7c2eRJC1ZmOFk/Recipe_DB";
    string recipeJsonSavePath = "Assets/Resources/Json/RecipeData.json";

    string saveItemSOPath = "Assets/Resources/Items";
    string saveRecipeSOPath = "Assets/Resources/Recipes";

#if UNITY_EDITOR
    [MenuItem("Window/DownLoader")]
    public static void ShowWindow()
    {
        GetWindow<DownLoader>("DownLoader");
    }
    private void OnGUI()
    {
        GUILayout.Label("JSON Downloader", EditorStyles.boldLabel);

        // URL 입력 필드
        itemDBUrl = EditorGUILayout.TextField("ItemURL", itemDBUrl);
        recipeDBUrl = EditorGUILayout.TextField("RecipeURL", recipeDBUrl);

        // 저장 경로 입력 필드
        itemJsonSavePath = EditorGUILayout.TextField("Item Json Save Path", itemJsonSavePath);
        recipeJsonSavePath = EditorGUILayout.TextField("Recipe Json Save Path", recipeJsonSavePath);

        // 다운로드 버튼
        if (GUILayout.Button("Download JSON"))
        {
            DownloadAndSaveJson(itemDBUrl,itemJsonSavePath);
            DownloadAndSaveJson(recipeDBUrl, recipeJsonSavePath);
        }

        GUILayout.Label("Json To SO", EditorStyles.boldLabel);

        saveItemSOPath = EditorGUILayout.TextField("SaveItemSOPath", saveItemSOPath);
        saveRecipeSOPath = EditorGUILayout.TextField("SaveRecipeSOPath", saveRecipeSOPath);
        if (GUILayout.Button("ItemConvert"))
        {
            ItemConvertJsonToSO();
        }
        if (GUILayout.Button("RecipeConvert"))
        {
            RecipeConvertJsonToSO();
        }
    }

    private System.Collections.IEnumerator DownloadJsonCoroutine(string url, string savePath)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllText(savePath, www.downloadHandler.text);
            Debug.Log($"JSON 저장 완료: {savePath}");
            AssetDatabase.Refresh(); // 에디터에 변경 사항 반영
        }
        else
        {
            Debug.LogError($"다운로드 실패: {www.error}");
        }
    }

    private void DownloadAndSaveJson(string url, string savePath)
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(DownloadJsonCoroutine(url,savePath));
    }

    void ItemConvertJsonToSO()
    {
        string jsonText = File.ReadAllText(itemJsonSavePath);
        List<ItemData> itemList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

        if (AssetDatabase.IsValidFolder(saveItemSOPath)) // 기존 데이터 삭제
        {
            FileUtil.DeleteFileOrDirectory(saveItemSOPath);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(saveItemSOPath))
        {
            Directory.CreateDirectory(saveItemSOPath);
        }

        foreach (ItemData data in itemList)
        {
            if (data == null)
            {
                Debug.LogError("data가 null입니다!");
                continue;
            }
            ItemData so = ScriptableObject.CreateInstance<ItemData>();
            so.id = data.id;
            so.name_en = data.name_en;
            so.name_kr = data.name_kr;
            so.item_type = data.item_type;
            so.attribute = data.attribute;
            so.target_range = data.target_range;
            so.effect_range = data.effect_range;
            so.effect_type = data.effect_type;
            so.effect_value = data.effect_value;
            so.effect_id = data.effect_id;
            so.effect_duration = data.effect_duration;
            so.effect_strength = data.effect_strength;
            so.mp_cost = data.mp_cost;
            so.max_stack = data.max_stack;
            so.icon_sprite = data.icon_sprite;
            so.iteminfo_kr = data.iteminfo_kr;
            so.Sprite = Resources.Load<Sprite>(data.icon_sprite);

            string assetPath = $"{saveItemSOPath}/{data.name_en}.asset";
            AssetDatabase.CreateAsset(so, assetPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Json변환 완료");
    }

    void RecipeConvertJsonToSO()
    {
        string jsonText = File.ReadAllText(recipeJsonSavePath);
        List<RecipeData> recipeList = JsonConvert.DeserializeObject<List<RecipeData>>(jsonText);

        if(AssetDatabase.IsValidFolder(saveRecipeSOPath))
        {
            FileUtil.DeleteFileOrDirectory(saveRecipeSOPath);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(saveRecipeSOPath))
        {
            Directory.CreateDirectory(saveRecipeSOPath);
        }

        foreach (RecipeData data in recipeList)
        {
            if (data == null)
            {
                Debug.LogError("data가 null입니다!");
                continue;
            }
            RecipeData so = ScriptableObject.CreateInstance<RecipeData>();
            so.id = data.id;
            so.recipe_name_kr = data.recipe_name_kr;
            so.recipe_name_en = data.recipe_name_en;
            so.output_item_id = data.output_item_id;
            so.output_amount = data.output_amount;
            so.material_1_item_id = data.material_1_item_id;
            so.material_1_amount = data.material_1_amount;
            so.material_2_item_id = data.material_2_item_id;
            so.material_2_amount = data.material_2_amount;
            so.material_3_item_id = data.material_3_item_id;
            so.material_3_amount = data.material_3_amount;
            so.mp_cost = data.mp_cost;
            so.unlock_condition = data.unlock_condition;
            so.efficiency_rating = data.efficiency_rating;
            so.icon_sprite_id = data.icon_sprite_id;

            string assetPath = $"{saveRecipeSOPath}/{data.recipe_name_en}.asset";
            AssetDatabase.CreateAsset(so, assetPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Json변환 완료");
    }


}
#endif
