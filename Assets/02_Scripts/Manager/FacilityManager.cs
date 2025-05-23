using System.IO;
using UnityEngine;

public class FacilityManager : Singleton<FacilityManager>
{
    [SerializeField] private PlayerFacilityData _playerFacilities = new PlayerFacilityData();
    
    private readonly string SAVE_FILE_NAME = "facility_data.json";
    private string SaveFilePath => Path.Combine(Application.dataPath, SAVE_FILE_NAME);
    
    private void Start()
    {
        Initialize();
        ResetFacilityData();
    }
    
    public void Initialize()
    {
        LoadFacilityData();
    }
    
    private void LoadFacilityData()
    {
        if (File.Exists(SaveFilePath))
        {
            string jsonData = File.ReadAllText(SaveFilePath);
            if (!string.IsNullOrEmpty(jsonData))
            {
                _playerFacilities = JsonUtility.FromJson<PlayerFacilityData>(jsonData);
            }
            else
            {
                _playerFacilities = new PlayerFacilityData();
                SetupDefaultFacilities();
            }
        }
        else
        {
            _playerFacilities = new PlayerFacilityData();
            SetupDefaultFacilities();
        }
    }
    
    private void SetupDefaultFacilities()
    {
        AddFacility(270001, 1, true);  // 길드는 처음부터 1레벨, 해금됨
        AddFacility(270002, 1, true);
        AddFacility(270003, 0, false);
        
        // 설정 후 저장
        SaveFacilityData();
    }
    
    private void AddFacility(int facilityId, int level, bool unlocked)
    {
        _playerFacilities.facilities.Add(new PlayerFacilityStatus
        {
            facility_id = facilityId,
            current_level = level,
            unlocked = unlocked
        });
    }
    
    public bool IsFacilityUnlocked(int facilityId)
    {
        var facility = _playerFacilities.facilities.Find(f => f.facility_id == facilityId);
        return facility != null && facility.unlocked;
    }
    
    public int GetFacilityLevel(int facilityId)
    {
        var facility = _playerFacilities.facilities.Find(f => f.facility_id == facilityId);
        return facility?.current_level ?? 0;
    }
    
    public void UnlockFacility(int facilityId)
    {
        var facility = _playerFacilities.facilities.Find(f => f.facility_id == facilityId);
        if (facility != null)
        {
            facility.unlocked = true;
            SaveFacilityData();
        }
    }
    
    public void UpgradeFacility(int facilityId)
    {
        var facility = _playerFacilities.facilities.Find(f => f.facility_id == facilityId);
        if (facility != null && facility.unlocked)
        {
            facility.current_level++;
            SaveFacilityData();
        }
    }
    
    private void SaveFacilityData()
    {
        string jsonData = JsonUtility.ToJson(_playerFacilities, true);
        File.WriteAllText(SaveFilePath, jsonData);
        Debug.Log($"시설 데이터가 저장되었습니다: {SaveFilePath}");
    }

    public void ResetFacilityData()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
        }
        
        _playerFacilities = new PlayerFacilityData();
        SetupDefaultFacilities();
        Debug.Log("시설 데이터가 초기화되었습니다.");
    }
}