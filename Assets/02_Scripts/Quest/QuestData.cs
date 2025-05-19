using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    main,
    sub,
}

[CreateAssetMenu(menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public int id;
    public string name_kr;
    public int biome_id;
    public int base_monster_level;
    public int level_per_floor;
    public float map_size_prob_small;
    public float map_size_prob_medium;
    public float map_size_prob_large;
    public bool is_fixed_map;
    public int dungeon_floor_count;
    public float trap_spawn_rate_small;
    public float trap_spawn_rate_medium;
    public float trap_spawn_rate_large;
    public int trap_spawn_max_count;
    public int trap_difficulty_level;
    public bool enable_first_room_boost;
    public QuestType quest_Type;
}
