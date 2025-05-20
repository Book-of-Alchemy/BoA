using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerFacilityStatus
{
    public string player_id;
    public int facility_id; 
    public int current_level;
    public bool unlocked;
}

[Serializable]
public class PlayerFacilityData
{
    public List<PlayerFacilityStatus> facilities = new List<PlayerFacilityStatus>();
} 