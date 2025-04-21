using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]
public class DamageItem : BaseItem
{


    public override void UseItem(List<CharacterStats> targets)
    {
        // 공격 로직
        if (targets.Count < 1)
        {
            Debug.Log("타겟이 없습니다.");
            return;
        }
        foreach (var target in targets)
            GameManager.Instance.PlayerTransform.Attack(target);
    }

}
