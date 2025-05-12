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
        int fastFoward = unit.actionCost;//action cost가 줄어든만큼 nextActionTime을 줄여줘야함
        unit.actionCostStat.AddModifier(modifier);
        fastFoward -= unit.actionCost;
        unit.nextActionTime -= fastFoward;
    }

    public override void OnExpire(CharacterStats target)
    {
        unit.actionCostStat.RemoveModifier(modifier);
    }
}
