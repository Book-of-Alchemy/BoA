using System;
using System.Collections.Generic;
public interface IImmunity
{
    public IEnumerable<Type> BlockedTypes { get; }
}

public class Immune : StatusEffect
{
    public IEnumerable<Type> BlockedTypes { get; }
    public Immune(StatusEffectData data, HashSet<Type> types)
    {
        this.data = data;
        BlockedTypes = types;
    }
    public Immune(StatusEffectData data, int value, int remainingTime, int tickInterval, HashSet<Type> types)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
        BlockedTypes = types;
    }
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

