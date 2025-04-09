using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class DownLoader : MonoBehaviour
{
#if UNITY_EDITOR
    [CustomEditor(typeof(DownLoader))]
    public class SheetDownButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DownLoader fnc = (DownLoader)target;
            if (GUILayout.Button("Create Prefabs"))
            {
                fnc.CreatePrefabs();
            }
        }
    }
    private void CreatePrefabs()
    {
#if UNITY_EDITOR
        string folderPath = "Assets/03_Prefabs/Item";
        string soPath = "Item";

        ItemData[] resources = Resources.LoadAll<ItemData>(soPath);

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/03_Prefabs", "Item");
        }

        for (int i = 0; i < resources.Length; i++)
        {
            string prefabsPath = $"{folderPath}/{resources[i].name_en}.prefab";

            GameObject itemObject = new GameObject(resources[i].name_en);


            // 기존 프리팹 덮어쓰기
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(itemObject, prefabsPath);
            EditorUtility.SetDirty(itemObject); //변경 사항 강제 적용, 반드시 하나했을때 개별적으로 적용시키기.
            DestroyImmediate(itemObject);
        }
        // 변경 사항 저장
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

#endif

}
#endif
