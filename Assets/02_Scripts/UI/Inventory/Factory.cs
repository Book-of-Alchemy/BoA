using System.Collections.Generic;
using UnityEngine;

public interface IItemCreate
{
    BaseItem CreateItem(Transform parent);
}

public class Factory
{
    private readonly Dictionary<Effect_Type, BaseItem> factories;

    public Factory()
    {
        factories = new Dictionary<Effect_Type, BaseItem>
        {
           { Effect_Type.Damage, new DamageItem() },
        };
    }

    public BaseItem CreateItem(Effect_Type effectType, Transform parent)
    {
        if (factories.TryGetValue(effectType, out var factory))
        {
            //return factory.UseItem(parent);
        }
        return null;
    }

}
