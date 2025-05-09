using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincible : Buff
{
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if(!shouldRegister) return;
        target.isInvincible = true;
    }

    public override void OnExpire(CharacterStats target)
    {
       target.isInvincible = false;
    }
}
