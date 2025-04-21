using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem
{
    public int item_id;
    public string name_en;
    public Item_Type item_type;
    public Attribute attribute;
    public int target_range;
    public int effect_range;
    public Effect_Type effect_type;
    //public Tag[] tags;
    public int effect_value;
    public int effect_id;
    public int effect_duration;
    public int effect_strength;
    public int mp_cost;
    public int max_stack;
    public string iteminfo_kr;
    public string icon_sprite;


    public virtual void Init(ItemData data)
    {
        item_id = data.id;
        name_en = data.name_en;
        item_type = data.item_type;
        attribute = data.attribute;
        target_range = data.target_range;
        effect_range = data.effect_range;
        effect_type = data.effect_type;
        //tags = data.tags;
        effect_id = data.effect_id;
        effect_duration = data.effect_duration;
        effect_strength = data.effect_strength;
        mp_cost = data.mp_cost;
        effect_value = data.effect_value;
        max_stack = data.max_stack;
        iteminfo_kr = data.iteminfo_kr;
        icon_sprite = data.icon_sprite;
    }

    public abstract void UseItem(List<CharacterStats> target);

}

