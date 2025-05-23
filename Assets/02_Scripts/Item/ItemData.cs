using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

[JsonConverter(typeof(StringEnumConverter))]
public enum Item_Type
{
    Consumable,
    Material,
    Special,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum Attribute
{
    None,
    Fire,
    Water,
    Cold,
    Lightning,
    Earth,
    Wind,
    Light,
    Dark,
    Oil,
    Non_elemental,
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
    Place_Environment_Tile
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
    Cold,
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
    Special,
    HP,
    MP,
    place_environment_tile,
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
    public string tag;
    public Tag[] tags;
    public int effect_value;
    public int effect_id;
    public int effect_duration;
    public int effect_strength;
    public int mp_cost;
    public int max_stack;
    public int price;
    public int tier;
    public string iteminfo_kr;
    public string icon_sprite;
    public Sprite sprite;
    public Sprite itemRangeSprite;
    public Sprite itemEffectRangeSprite;

    [Header("드랍생성용 프리팹")]
    public GameObject itemPrefab;    // 새로 추가됨(04.29 이성재)
}

