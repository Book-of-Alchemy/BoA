using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class FacilityDatabaseCreator : EditorWindow
{
    [MenuItem("Tools/Create Facility Database")]
    public static void CreateFacilityDatabase()
    {
        // 저장 경로 확인 및 생성
        string directory = "Assets/08_ScriptableObjects/Facility";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 데이터베이스 생성
        FacilityDataBase database = ScriptableObject.CreateInstance<FacilityDataBase>();
        string assetPath = $"{directory}/FacilityDataBase.asset";
        
        // 이미 존재하는지 확인
        if (AssetDatabase.LoadAssetAtPath<FacilityDataBase>(assetPath) != null)
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "경고", 
                "FacilityDataBase.asset이 이미 존재합니다. 덮어쓰시겠습니까?", 
                "예", "아니오");
                
            if (!overwrite)
                return;
        }
        
        // 데이터 저장
        AssetDatabase.CreateAsset(database, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // 저장 확인
        EditorUtility.DisplayDialog("성공", "시설 데이터베이스가 생성되었습니다.", "확인");
        
        // 생성된 데이터 선택
        Selection.activeObject = database;
    }
}
#endif 