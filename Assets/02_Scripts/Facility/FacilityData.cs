using UnityEngine;

[CreateAssetMenu(menuName = "Facility/New FacilityData")]
public class FacilityData : ScriptableObject
{
    public int id;
    public string name_kr;
    public int current_level;
    public bool unlocked;
    public string description;
    public Sprite icon;
    public GameObject prefab;
    
    public int upgrade_cost_base;
    public int upgrade_cost_multiplier;
    public int max_level = 3;
} 