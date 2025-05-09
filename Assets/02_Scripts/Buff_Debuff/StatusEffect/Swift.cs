using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swift : StatBuff
{
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        modifier = new StatModifier("Swift", -value/2, ModifierType.Precent);
        int fastFoward = target.unitBase.actionCost;//action cost가 줄어든만큼 nextActionTime을 줄여줘야함
        target.unitBase.actionCostStat.AddModifier(modifier);
        fastFoward -= target.unitBase.actionCost;
        target.unitBase.nextActionTime -= fastFoward;
    }

    public override void OnExpire(CharacterStats target)
    {
        target.unitBase.actionCostStat.RemoveModifier(modifier);
    }
}
