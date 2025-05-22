using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectrofiedWaterTile : TileEffect, IGround, IWater,IExpirable
{
    public override EnvironmentType EnvType => EnvironmentType.Electrofied_Water;

    private int leftTime = 50;
    public int LeftTime { get => leftTime; set => leftTime = value; }
    public override void PerformAction()
    {
        if (LeftTime <= 0)
        {
            Expire();
            return;
        }

        StatusEffectFactory.CreateEffect(220012, CurTile.CharacterStatsOnTile);
        Burn burn = null;
        LeftTime -= ActionCost;
        if (CurTile.CharacterStatsOnTile != null)
        {
            burn = CurTile.CharacterStatsOnTile.activeEffects
                .OfType<Burn>()
                .FirstOrDefault();
        }

        if (burn != null)
        {
            burn.Cleanse();
        }
    }

    public void Expire()
    {
        EnvironmentalFactory.Instance.ReturnTileEffect(this);
    }
}
