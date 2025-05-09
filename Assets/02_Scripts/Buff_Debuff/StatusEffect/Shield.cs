using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Buff
{
    public override void OnApply(CharacterStats target)
    {
        foreach (var effect in target.activeEffects.ToArray())
        {
            if (effect.GetType() == this.GetType())
            {
                if (effect.value < value)//구 effect value 가 낮다면 파기후 신 effect 적용
                {
                    effect.OnExpire(target);
                    target.activeEffects.Remove(effect);
                    break;
                }
                shouldRegister = false;
            }
        }
        if (!shouldRegister) return;
        modifier = new StatModifier("AttackIncrese", value, ModifierType.Flat);
        target.statBlock.AddModifier(StatType.Attack, modifier);

    }
}
