using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : BaseItem
{
    public override void UseItem(ItemData data)
    {
        GameManager.Instance.PlayerTransform.Heal(data.effect_value);
        FinishUse();
        Destroy(this.gameObject);
    }

}
