using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/New EnemyData")]
public class EnemyData : PrefabWithId
{
    public string name_kr;
    public int biome_id;
    public int sight;
    public int attack_range;
    public int min_level;
    public int max_level;
    public int base_hp;
    public int hp_per_level;
    public int base_atk;
    public int atk_per_level;
    public int base_def;
    public int def_per_level;
    public float resist_fire;
    public float resist_water;
    public float resist_cold;
    public float resist_wind;
    public float resist_lightning;
    public float resist_earth;
    public float resist_light;
    public float resist_dark;
}