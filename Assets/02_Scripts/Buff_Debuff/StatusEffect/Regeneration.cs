using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regeneration : Buff
{
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if(!shouldRegister) return;

    }

    public override void Tick(CharacterStats target)
    {
        target.Heal(value);
    }
}
