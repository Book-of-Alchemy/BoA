using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    main,
    sub,
}
public enum ObjectType
{
    ActivateEvent,
    ReachLastEscape,
    DefeatMonsterType,
    DefeatMonsterTotal,
    DefeatBoss,
    WithinTurnLimit,
    DamageTakenLimit,
    AvoidEnvironmentalDamage,
    ExploreAllRooms,
    ReachSpecificRoom,
}

public enum Reward
{
    recipe,
    artifact,
    facility,
    none,
}

[CreateAssetMenu(menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public int id;
    public string quest_name_kr;
    public string quest_name_en;
    public int biome_id;
    public int base_monster_level;
    public int level_per_floor;
    public bool is_fixed_map;
    public int dungeon_floor_count;
    public QuestType quest_Type;
    public ObjectType main_object_type;
    public string main_object_text_kr;
    public Reward reward1;
    public Reward reward2;
    public Reward reward3;
    public int reward_gold_amount;
    public string client;
    public string descriptiontxt;
}
