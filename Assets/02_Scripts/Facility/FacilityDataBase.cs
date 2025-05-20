using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Facility/FacilityDataBase")]
public class FacilityDataBase : ScriptableObject
{
    public List<FacilityData> facilityData;
    
    // 편의를 위한 메서드
    public FacilityData GetFacilityById(int id)
    {
        return facilityData.Find(f => f.id == id);
    }
} 