using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trap/New Trap")]
public class TrapData : ScriptableObject
{
    public int id;
    public string name_kr;
    public string name_en;
    public int damage;
    public int effect_range;
    public int effect_id;
    public int effect_value;
    public int effect_duration;
    public bool detected_by_default;
    public int min_spawn_floor;
    public int max_spawn_floor;
    public int spawn_weight_easy;
    public int spawn_weight_hard;
    public bool special_only;
    public GameObject trapPrefab;
}
