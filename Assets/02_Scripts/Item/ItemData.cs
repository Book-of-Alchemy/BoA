using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Item_Type
{
    Consumable,
    Material,
}

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

public enum Effect_Type
{
    Damage,
    Heal,
    Buff,
    Debuff,
    Move,
    None,
}

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
    public string item_id;
    public string name_en;
    public string name_kr;
    public Item_Type item_type;
    public Attribute attribute;
    public int target_range;
    public int effect_range;
    public Effect_Type effect_type;
    public Tag[] tags;
    public int effect_value;
    public int max_stack;
    public string iteminfo_kr;
    public string icon_sprite;
}
