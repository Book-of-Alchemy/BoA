using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Asleep : Debuff
{
    UnitBase unit;
    CharacterStats stats;
    public override void Tick(CharacterStats target)
    {
        if (unit == null || remainingTime <= tickInterval)
            return;

        unit.nextActionTime += 10;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        stats = target;
        unit = target.unitBase;
        target.OnTakeDamage += WakeUp;

        Tick(target);
    }

    public override void OnExpire(CharacterStats target)
    {
        target.OnTakeDamage -= WakeUp;
    }

    void WakeUp()
    {
        OnExpire(stats);
        stats.activeEffects.Remove(this);
    }
}
