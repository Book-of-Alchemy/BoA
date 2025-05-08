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

    }

    public virtual void Cleanse()
    {
        OnExpire(target);
        target.activeEffects.Remove(this);
    }
}
