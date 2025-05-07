using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Debuff : StatusEffect
{
    public override void OnApply(CharacterStats target) 
    {
        if(target.hasImmunityToAll || target.HasImmunity(this.GetType()))
        {
            target.activeEffects.Remove(this);
            return;
        }
    }
}
