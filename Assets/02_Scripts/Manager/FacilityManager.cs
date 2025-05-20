using System.Collections.Generic;
using UnityEngine;

public class FacilityManager : Singleton<FacilityManager>
{
    [SerializeField] private PlayerFacilityData _playerFacilities = new PlayerFacilityData();
    [SerializeField] private string _currentPlayerId = "100001";
    
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
        string jsonData = PlayerPrefs.GetString("facility_data", "");
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
            player_id = _currentPlayerId,
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
        string jsonData = JsonUtility.ToJson(_playerFacilities);
        PlayerPrefs.SetString("facility_data", jsonData);
        PlayerPrefs.Save();
    }

    public void ResetFacilityData()
    {
        PlayerPrefs.DeleteKey("facility_data");
        _playerFacilities = new PlayerFacilityData();
        SetupDefaultFacilities();
        Debug.Log("시설 데이터가 초기화되었습니다.");
    }
}