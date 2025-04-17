using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageItem : BaseItem
{
    public override void UseItem(CharacterStats[] targets)
    {
        // 공격 로직
        if (targets.Length < 1)
        {
            Debug.Log("타겟이 없습니다.");
            return;
        }
        foreach(var target in targets)
            target.TakeDamage((float)effect_value);
    }

}
