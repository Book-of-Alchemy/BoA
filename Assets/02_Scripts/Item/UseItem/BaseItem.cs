using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseItem
{
    private string item_id;
    private string name_en;
    private Item_Type item_type;
    private Attribute attribute;
    private int target_range;
    private int effect_range;
    private Effect_Type effect_type;
    private Tag[] tags;
    private int effect_value;
    private int max_stack;
    private string iteminfo_kr;
    private string icon_sprite;


    public void Init(ItemData data)
    {
        item_id = data.item_id;
        name_en = data.name_en;
        item_type = data.item_type;
        attribute = data.attribute;
        target_range = data.target_range;
        effect_range = data.effect_range;
        effect_type = data.effect_type;
        tags = data.tags;
        effect_value = data.effect_value;
        max_stack = data.max_stack;
        iteminfo_kr = data.iteminfo_kr;
        icon_sprite = data.icon_sprite;
    }
    /// <summary>
    /// 사용시 플레이어나 적의스텟에 접근해서 조정하는 방식으로 하면 될듯함
    /// </summary>
    public void UseItem()
    {
        switch (effect_type)
        {
            case Effect_Type.Damage:
                Damage();
                break;
            case Effect_Type.Heal:
                Heal();
                break;
            case Effect_Type.Buff:
                Buff();
                break;
            case Effect_Type.Debuff:
                Debuff();
                break;
            case Effect_Type.Move:
                Move();
                break;
            case Effect_Type.None:
                // 사용불가
                break;
        }
    }

    private void Damage()
    {
        //TODO 공격 로직
    }
    private void Heal()
    {
        //TODO 힐 로직
    }
    private void Buff()
    {
        //TODO 버프 로직
    }
    private void Debuff()
    {
        //TODO 디버스 로직
    }
    private void Move()
    {
        //TODO 강제이동 로직
    }
}

