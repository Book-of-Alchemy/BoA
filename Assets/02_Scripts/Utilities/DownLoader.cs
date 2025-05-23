using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using Newtonsoft.Json;
using System.Linq;
using System;




public class DownLoader : EditorWindow
{
    string itemDBUrl = "https://opensheet.elk.sh/1kt6Cg9_sC3G4imRU9_b0_o0j-XP5KUCrDXyvX__BdH4/Item_DB";
    string itemJsonSavePath = "Assets/Resources/Json/ItemData.json";
    string recipeDBUrl = "https://opensheet.elk.sh/1CPbtP4IkSU3za-8Y-BjNnG6_fGdG-z7c2eRJC1ZmOFk/Recipe_DB";
    string recipeJsonSavePath = "Assets/Resources/Json/RecipeData.json";
    string statusEffectDBUrl = "https://opensheet.elk.sh/1ILhdJKFLc1kmaMduHtTpcZEXAzIarNJo-YM6jcW-KIM/Buff%2FDebuff_DB";
    string statusEffectJsonSavePath = "Assets/Resources/Json/StatusEffect.json";
    string artifactDBUrl = "https://opensheet.elk.sh/16BPAGJg1d_g6m4tCxN9aJWzjWwzPmHyOaTcqg1uclhI/Artifact_DB";
    string artifactJsonSavePath = "Assets/Resources/Json/Artifact.json";
    string questDBUrl = "https://opensheet.elk.sh/1udhK2PAP126vzsQycyXA_tEmNqTA3VlwmfU--CHqHd0/Quest_DB.csv";
    string questJsonSavePath = "Assets/Resources/Json/Quest.json";
    string researchDBUrl = "https://opensheet.elk.sh/18J2rdxLQUZwBIhTPIzRW7roD1Vwwt9L14FHOvudFGno/HOK_DB";
    string researchJsonSavePath = "Assets/Resources/Json/Research.json";
    string soundDBUrl = "https://opensheet.elk.sh/1opKr37fwkgj0liXqkWhx_pYQEsf97agKquDpO236kgk/Audio_DB";
    string soundJsonSavePath = "Assets/Resources/Json/Sound.json";

    string saveItemSOPath = "Assets/Resources/Items";
    string saveRecipeSOPath = "Assets/Resources/Recipes";
    string saveStatusEffectSOPath = "Assets/08_ScriptableObjects/StatusEffect";
    string saveArtifactSOPath = "Assets/08_ScriptableObjects/Artifact";
    string saveQuestSOPath = "Assets/08_ScriptableObjects/Quest";
    string saveResearchSOPath = "Assets/08_ScriptableObjects/Research";
    string saveSoundSOPath = "Assets/08_ScriptableObjects/Sound";

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
        statusEffectDBUrl = EditorGUILayout.TextField("StatusEffectURL", statusEffectDBUrl);
        artifactDBUrl = EditorGUILayout.TextField("ArtifactDBUrl", artifactDBUrl);
        questDBUrl = EditorGUILayout.TextField("QuestDBUrl", questDBUrl);
        researchDBUrl =EditorGUILayout.TextField("ResearchDBUrl", researchDBUrl);
        soundDBUrl = EditorGUILayout.TextField("SoundDBUrl", soundDBUrl);


        // 저장 경로 입력 필드
        itemJsonSavePath = EditorGUILayout.TextField("Item Json Save Path", itemJsonSavePath);
        recipeJsonSavePath = EditorGUILayout.TextField("Recipe Json Save Path", recipeJsonSavePath);
        statusEffectJsonSavePath = EditorGUILayout.TextField("StatusEffect Json Save Path", statusEffectJsonSavePath);
        artifactJsonSavePath = EditorGUILayout.TextField("Artifact Json Save Path", artifactJsonSavePath);
        questJsonSavePath = EditorGUILayout.TextField("Quest Json Save Path", questJsonSavePath);
        researchJsonSavePath = EditorGUILayout.TextField("Research Json Save Path", researchJsonSavePath);
        soundJsonSavePath = EditorGUILayout.TextField("Sound Json Save Path", soundJsonSavePath);


        // 다운로드 버튼
        if (GUILayout.Button("Download JSON"))
        {
            DownloadAndSaveJson(itemDBUrl,itemJsonSavePath);
            DownloadAndSaveJson(recipeDBUrl, recipeJsonSavePath);
            DownloadAndSaveJson(statusEffectDBUrl, statusEffectJsonSavePath);
            DownloadAndSaveJson(artifactDBUrl, artifactJsonSavePath);
            DownloadAndSaveJson(questDBUrl, questJsonSavePath);
            DownloadAndSaveJson(researchDBUrl, researchJsonSavePath);
            DownloadAndSaveJson(soundDBUrl, soundJsonSavePath);
        }

        GUILayout.Label("Json To SO", EditorStyles.boldLabel);

        saveItemSOPath = EditorGUILayout.TextField("SaveItemSOPath", saveItemSOPath);
        saveRecipeSOPath = EditorGUILayout.TextField("SaveRecipeSOPath", saveRecipeSOPath);
        saveStatusEffectSOPath = EditorGUILayout.TextField("SaveStatusEffectSOPath", saveStatusEffectSOPath);
        saveArtifactSOPath = EditorGUILayout.TextField("SaveArtifactSOPath", saveArtifactSOPath);
        saveQuestSOPath = EditorGUILayout.TextField("SaveQuestSOPath", saveQuestSOPath);
        saveResearchSOPath = EditorGUILayout.TextField("SaveResearchSoPath", saveResearchSOPath);
        saveSoundSOPath = EditorGUILayout.TextField("SaveSoundSOPath", saveSoundSOPath);

        if (GUILayout.Button("ItemConvert"))
        {
            ItemConvertJsonToSO();
        }
        if (GUILayout.Button("RecipeConvert"))
        {
            RecipeConvertJsonToSO();
        }
        if (GUILayout.Button("StatusEffectConvert"))
        {
            StatusEffectConvertJsonToSO();
        }
        if (GUILayout.Button("ArtifactConvert"))
        {
            ArtifactConvertJsonToSO();
        }
        if(GUILayout.Button("QuestConvert"))
        {
            QuestConvertJsonToSO();
        }
        if(GUILayout.Button("ResearchConvert"))
        {
            ResearchConvertJsonToSO();
        }
        if(GUILayout.Button("SoundConvert"))
        {
            SoundConvertJsonToSO();
        }

    }

    private System.Collections.IEnumerator DownloadJsonCoroutine(string url, string savePath)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllText(savePath, www.downloadHandler.text);
            AssetDatabase.Refresh(); // 에디터에 변경 사항 반영
        }
        else
        {
        }
    }

    private void DownloadAndSaveJson(string url, string savePath)
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(DownloadJsonCoroutine(url,savePath));
    }

    /// <summary>
    /// 아이템json => SO변환
    /// </summary>
    /// <exception cref="Exception"></exception>
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
            so.price = data.price;
            so.tier = data.tier;
            so.icon_sprite = data.icon_sprite;
            so.iteminfo_kr = data.iteminfo_kr;
            so.sprite = Resources.Load<Sprite>(data.icon_sprite);
            if (data.tag != null)
            {
                so.tags = data.tag.Split(',')
                    .Select(s => s.Trim())
                    .Select(s => Enum.TryParse<Tag>(s, true, out var t) ? t : throw new Exception($"Invalid tag: {s}"))
                    .ToArray();
            }
            else
                return;
            so.itemRangeSprite = Resources.Load<Sprite>($"Image/RangeIcon/range_{data.target_range}");
            so.itemEffectRangeSprite = Resources.Load<Sprite>($"Image/RangeIcon/range_{data.effect_range}");


            string assetPath = $"{saveItemSOPath}/{data.name_en}.asset";
            AssetDatabase.CreateAsset(so, assetPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 레시피 json => SO변환
    /// </summary>
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
    }

    /// <summary>
    /// 버프디버프 json => SO변환
    /// </summary>
    void StatusEffectConvertJsonToSO()
    {
        string jsonText = File.ReadAllText(statusEffectJsonSavePath);
        List<StatusEffectData> dataList = JsonConvert.DeserializeObject<List<StatusEffectData>>(jsonText);

        if (AssetDatabase.IsValidFolder(saveStatusEffectSOPath))
        {
            FileUtil.DeleteFileOrDirectory(saveStatusEffectSOPath);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(saveStatusEffectSOPath))
        {
            Directory.CreateDirectory(saveStatusEffectSOPath);
        }

        foreach (var data in dataList)
        {
            if (data == null)
            {
                continue;
            }
            StatusEffectData so = ScriptableObject.CreateInstance<StatusEffectData>();
            so.id = data.id;
            so.name_kr = data.name_kr;
            so.type = data.type;
            so.effect_category = data.effect_category;
            so.icon_sprite = data.icon_sprite;
            so.description = data.description;
            so.duration_type = data.duration_type;
            so.isStackable = data.isStackable;
            so.special_note = data.special_note;
            so.icon = Resources.Load<Sprite>(data.icon_sprite);

            string assetPath = $"{saveStatusEffectSOPath}/{data.name_kr}.asset";
            AssetDatabase.CreateAsset(so, assetPath);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 아티팩트 json => SO변환
    /// </summary>
    void ArtifactConvertJsonToSO()
    {
        string jsonText = File.ReadAllText(artifactJsonSavePath);
        List<ArtifactData> dataList = JsonConvert.DeserializeObject<List<ArtifactData>>(jsonText);

        if (AssetDatabase.IsValidFolder(saveArtifactSOPath))
        {
            FileUtil.DeleteFileOrDirectory(saveArtifactSOPath);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(saveArtifactSOPath))
        {
            Directory.CreateDirectory(saveArtifactSOPath);
        }

        foreach (var data in dataList)
        {
            if (data == null)
            {
                continue;
            }
            ArtifactData so = ScriptableObject.CreateInstance<ArtifactData>();
            so.id = data.id;
            so.name_kr = data.name_kr;
            so.name_en = data.name_en;
            so.description = data.description;
            so.icon_sprite = Resources.Load<Sprite>(data.icon_sprite_id);
            so.rarity = data.rarity;

            string assetPath = $"{saveArtifactSOPath}/{data.name_en}.asset";
            AssetDatabase.CreateAsset(so, assetPath);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 퀘스트json => SO변환
    /// </summary>
    void QuestConvertJsonToSO()
    {
        string jsonText = File.ReadAllText(questJsonSavePath);
        List<QuestData> dataList = JsonConvert.DeserializeObject<List<QuestData>>(jsonText);

        if (AssetDatabase.IsValidFolder(saveQuestSOPath))
        {
            FileUtil.DeleteFileOrDirectory(saveQuestSOPath);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(saveQuestSOPath))
        {
            Directory.CreateDirectory(saveQuestSOPath);
        }

        foreach (var data in dataList)
        {
            if (data == null)
            {
                continue;
            }
            QuestData so = ScriptableObject.CreateInstance<QuestData>();
            so.id = data.id;
            so.quest_name_kr = data.quest_name_kr;
            so.quest_name_en = data.quest_name_en;
            so.biome_id = data.biome_id;
            so.base_monster_level = data.base_monster_level;
            so.level_per_floor = data.level_per_floor;
            so.is_fixed_map = data.is_fixed_map;
            so.dungeon_floor_count = data.dungeon_floor_count;
            so.quest_Type = data.quest_Type;
            so.main_object_type = data.main_object_type;
            so.main_object_text_kr = data.main_object_text_kr;
            so.reward1 = data.reward1;
            so.reward2 = data.reward2;
            so.reward3 = data.reward3;
            so.reward_gold_amount = data.reward_gold_amount;
            so.client = data.client;
            so.descriptiontxt = data.descriptiontxt;

            string assetPath = $"{saveQuestSOPath}/{data.quest_name_en}.asset";
            AssetDatabase.CreateAsset(so, assetPath);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 연구json => SO변환
    /// </summary>
    void ResearchConvertJsonToSO()
    {
        string jsonText = File.ReadAllText(researchJsonSavePath);
        List<ResearchData> dataList = JsonConvert.DeserializeObject<List<ResearchData>>(jsonText);

        if (AssetDatabase.IsValidFolder(saveResearchSOPath))
        {
            FileUtil.DeleteFileOrDirectory(saveResearchSOPath);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(saveResearchSOPath))
        {
            Directory.CreateDirectory(saveResearchSOPath);
        }

        foreach (var data in dataList)
        {
            if (data == null)
            {
                continue;
            }
            ResearchData so = ScriptableObject.CreateInstance<ResearchData>();
            so.id = data.id;
            so.name_kr = data.name;
            so.name_en = data.name_en;
            so.description_kr = data.description_kr;
            so.stat_type = data.stat_type;
            so.stat_value = data.stat_value;
            so.required_research_id = data.required_research_id;
            so.research_cost = data.research_cost;
            so.max_level = data.max_level;
            so.icon_sprite = Resources.Load<Sprite>(data.icon_sprite_id);

            string assetPath = $"{saveResearchSOPath}/{data.name_en}.asset";
            AssetDatabase.CreateAsset(so, assetPath);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void SoundConvertJsonToSO()
    {
        string jsonText = File.ReadAllText(soundJsonSavePath);
        List<SoundData> dataList = JsonConvert.DeserializeObject<List<SoundData>>(jsonText);

        if (AssetDatabase.IsValidFolder(saveSoundSOPath))
        {
            FileUtil.DeleteFileOrDirectory(saveSoundSOPath);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(saveSoundSOPath))
        {
            Directory.CreateDirectory(saveSoundSOPath);
        }

        foreach (var data in dataList)
        {
            if (data == null)
            {
                continue;
            }
            SoundData so = ScriptableObject.CreateInstance<SoundData>();
            so.id = data.id;
            so.type = data.type;
            so.clip = Resources.Load<AudioClip>(data.audioClipPath);

            string assetPath = $"{saveSoundSOPath}/{data.id}.asset";
            AssetDatabase.CreateAsset(so, assetPath);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


}
#endif
