using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTrap : TrapBase
{
    public override void Action()
    {
        effectTiles = TileUtility.GetRangedTile(tile.curLevel, tile, trapData.effect_range, true);
        DamageType damageType = (DamageType)(trapData.effect_value == -1 ? 0 : trapData.effect_value);

        foreach (var effect in effectTiles)
        {
            if (effect.CharacterStatsOnTile == null || effect == null)
                return;
            effect.CharacterStatsOnTile.TakeDamage(DamageCalculator.CalculateDamage(effect.CharacterStatsOnTile, trapData.damage, damageType));
        }
    }
}
