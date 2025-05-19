using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OilTile : TileEffect, IGround, IWater
{
    public override EnvironmentType EnvType => EnvironmentType.Oil;
    public override void PerformAction()
    {
        Burn burn = null;
        if (CurTile.CharacterStatsOnTile != null)
        {
            burn = CurTile.CharacterStatsOnTile.activeEffects
                .OfType<Burn>()
                .FirstOrDefault();
        }

        if (burn != null)
        {
            var damage = new DamageInfo(burn.value,DamageType.Fire,null,null);
            TileRuleProccessor.ProcessTileReactions(damage, CurTile);
        }
    }
}
