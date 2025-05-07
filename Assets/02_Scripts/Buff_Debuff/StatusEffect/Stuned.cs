using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuned : Debuff
{
    UnitBase unit;
    public override void Tick(CharacterStats target)
    {
        if (unit == null || remainingTime <= tickInterval)
            return;

        unit.nextActionTime += 10;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        unit = target.GetComponent<UnitBase>();
        Tick(target);
    }
}
