using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Debuff : StatusEffect
{
    protected StatModifier modifier;
    protected CharacterStats target;
    public override void OnApply(CharacterStats target) 
    {
        if(target.hasImmunityToAll || target.HasImmunity(this.GetType()))
        {
            shouldRegister = false;
            return;
        }

        if(!IsStackable)
        {
            foreach(var effect in target.activeEffects.ToArray())
            {
                if(effect.GetType() == this.GetType())
                {
                    if(effect.value < value)//구 effect value 가 낮다면 파기후 신 effect 적용
                    {
                        effect.OnExpire(target);
                        target.activeEffects.Remove(effect);
                        break;
                    }

                    if(effect.remainingTime < remainingTime)//신 effect value가 더 낮지만 remaining이 더 많다면 신 remaining 적용
                    {
                        effect.remainingTime = remainingTime;
                    }

                    shouldRegister = false;
                }
            }
            
        }

    }

    public virtual void Cleanse()
    {
        OnExpire(target);
        target.activeEffects.Remove(this);
    }
}
