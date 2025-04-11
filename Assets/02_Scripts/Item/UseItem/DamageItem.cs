using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageItem : BaseItem
{
    public override void UseItem()
    {
        // 공격 로직
        Debug.Log("공격 뿌직");
    }

}
