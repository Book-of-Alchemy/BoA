using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class FacilityDataCreator : EditorWindow
{
    private string facilityName = "New Facility";
    private int id = 270001;
    private string description = "시설 설명";
    private int upgradeBaseCost = 100;
    private int upgradeMultiplier = 150;
    private int maxLevel = 3;
    private bool startUnlocked = false;
    private Sprite icon;
    private GameObject prefab;

    [MenuItem("Tools/Facility Data Creator")]
    public static void ShowWindow()
    {
        GetWindow<FacilityDataCreator>("시설 데이터 생성기");
    }

    private void OnGUI()
    {
        GUILayout.Label("시설 데이터 생성", EditorStyles.boldLabel);
        
        facilityName = EditorGUILayout.TextField("시설 이름", facilityName);
        id = EditorGUILayout.IntField("ID (27xxxx)", id);
        description = EditorGUILayout.TextField("설명", description);
        upgradeBaseCost = EditorGUILayout.IntField("기본 업그레이드 비용", upgradeBaseCost);
        upgradeMultiplier = EditorGUILayout.IntField("업그레이드 비용 증가율", upgradeMultiplier);
        maxLevel = EditorGUILayout.IntField("최대 레벨", maxLevel);
        startUnlocked = EditorGUILayout.Toggle("시작부터 해금", startUnlocked);
        icon = (Sprite)EditorGUILayout.ObjectField("아이콘", icon, typeof(Sprite), false);
        prefab = (GameObject)EditorGUILayout.ObjectField("프리팹", prefab, typeof(GameObject), false);

        // ID 유효성 검사
        if (id < 270000 || id > 279999)
        {
            EditorGUILayout.HelpBox("ID는 27로 시작하는 6자리 숫자여야 합니다.", MessageType.Warning);
        }

        if (GUILayout.Button("시설 데이터 생성"))
        {
            CreateFacilityData();
        }
    }

    private void CreateFacilityData()
    {
        // 유효성 검사
        if (id < 270000 || id > 279999)
        {
            EditorUtility.DisplayDialog("오류", "ID는 27로 시작하는 6자리 숫자여야 합니다.", "확인");
            return;
        }

        // 데이터 생성
        FacilityData facilityData = ScriptableObject.CreateInstance<FacilityData>();
        facilityData.id = id;
        facilityData.name_kr = facilityName;
        facilityData.description = description;
        facilityData.upgrade_cost_base = upgradeBaseCost;
        facilityData.upgrade_cost_multiplier = upgradeMultiplier;
        facilityData.max_level = maxLevel;
        facilityData.unlocked = startUnlocked;
        facilityData.icon = icon;
        facilityData.prefab = prefab;

        // 저장 경로 확인 및 생성
        string directory = "Assets/08_ScriptableObjects/Facility";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 데이터 저장
        string assetPath = $"{directory}/{facilityName}.asset";
        AssetDatabase.CreateAsset(facilityData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 저장 확인
        EditorUtility.DisplayDialog("성공", $"{facilityName} 시설 데이터가 생성되었습니다.", "확인");
        
        // 생성된 데이터 선택
        Selection.activeObject = facilityData;
    }
}
#endif 