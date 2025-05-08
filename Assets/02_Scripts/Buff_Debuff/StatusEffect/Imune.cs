using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IImunity
{
    public IEnumerable<Type> BlockedTypes { get; }
}

public class Imune : StatusEffect, IImunity
{
    public IEnumerable<Type> BlockedTypes { get; }

    public override void OnApply(CharacterStats target)
    {
        foreach (var effect in target.activeEffects.ToArray())
        {
            foreach (Type type in BlockedTypes)
            {
                if (effect.GetType() == type)
                {
                    effect.OnExpire(target);
                    target.activeEffects.Remove(effect);
                }
            }
        }
    }

}

