using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItem : BaseItem
{
    public override void UseItem(ItemData data)
    {
        StatusEffectFactory.CreateEffect(data.effect_id, data.effect_strength, data.effect_duration, 10, GameManager.Instance.PlayerTransform);
    }
}
