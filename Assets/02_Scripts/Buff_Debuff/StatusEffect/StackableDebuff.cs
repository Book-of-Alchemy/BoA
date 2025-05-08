using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStackable
{
    public int StackCount { get; set; }
}
public class StackableDebuff<T>  : Debuff, IStackable where T : Debuff, IStackable
{
    protected int stackCount;
    public int StackCount { get => stackCount; set => stackCount = value; }
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        T exist = null;
        foreach(var effect in target.activeEffects)
        {
            if(effect.GetType() == typeof(T))
            {
                exist = effect as T;
                break;
            }
        }

        if (exist != null)
        {
            shouldRegister = false;
            exist.StackCount++;
            exist.remainingTime += remainingTime;
            return;
        }

        StackCount++;
    }

    
}
    
    

