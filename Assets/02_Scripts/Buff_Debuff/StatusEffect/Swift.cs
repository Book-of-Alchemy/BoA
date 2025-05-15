using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swift : StatBuff
{
    UnitBase unit;
    public Swift(StatusEffectData data)
    {
        this.data = data;
    }
    public Swift(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        unit = target.unitBase;
        modifier = new StatModifier("Swift", -value/2, ModifierType.Precent);
        int fastFoward = unit.ActionCost;//action cost가 줄어든만큼 nextActionTime을 줄여줘야함
        unit.ActionCostStat.AddModifier(modifier);
        fastFoward -= unit.ActionCost;
        unit.NextActionTime -= fastFoward;
    }

    public override void OnExpire(CharacterStats target)
    {
        unit.ActionCostStat.RemoveModifier(modifier);
    }
}
