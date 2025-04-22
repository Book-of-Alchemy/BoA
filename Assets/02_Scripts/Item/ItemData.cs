using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

[JsonConverter(typeof(StringEnumConverter))]
public enum Item_Type
{
    Consumable,
    Material,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum Attribute
{
    Non_elemental,
    Fire,
    Water,
    Ice,
    Wind,
    Lightning,
    Earth,
    Light,
    Dark,
    None,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum Effect_Type
{
    Damage,
    Heal,
    Buff,
    Debuff,
    Move,
    Place_Trap,
    None,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum Tag
{
    Throw,
    Scroll,
    Trap,
    Potion,
    Non_elemental,
    Fire,
    Water,
    Ice,
    Wind,
    Lightning,
    Earth,
    Light,
    Dark,
    AOE,
    Single_target,
    Non_dot,
    Dot,
    Debuff,
    Buff,
    Heal,
    Move,
    Material,
    Consumable,
    Supply,
    Equip,
    Hat,
    Cloth,
    Neckless,
    Glove,
    Shoes,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public int id;
    public string name_en;
    public string name_kr;
    public Item_Type item_type;
    public Attribute attribute;
    public int target_range;
    public int effect_range;
    public Effect_Type effect_type;
    ///public Tag[] tags;
    public int effect_value;
    public int effect_id;
    public int effect_duration;
    public int effect_strength;
    public int mp_cost;
    public int max_stack;
    public string iteminfo_kr;
    public string icon_sprite;
    public Sprite Sprite;
}

