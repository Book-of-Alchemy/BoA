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
    public int resist_fire;
    public int resist_water;
    public int resist_cold;
    public int resist_wind;
    public int resist_lightning;
    public int resist_earth;
    public int resist_light;
    public int resist_dark;
    public bool isBoss;
}