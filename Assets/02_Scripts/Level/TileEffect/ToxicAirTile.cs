using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicAirTile : TileEffect, IAir, IExpirable
{
    public override EnvironmentType EnvType => EnvironmentType.Fog;
    private int leftTime = 50;
    public int LeftTime { get => leftTime; set => leftTime = value; }
    public override void PerformAction()
    {
        if (LeftTime <= 0)
        {
            Expire();
            return;
        }

        StatusEffectFactory.CreateEffect(220010, CurTile.CharacterStatsOnTile);
        LeftTime -= ActionCost;
    }

    public void Expire()
    {
        EnvironmentalFactory.Instance.ReturnTileEffect(this);
    }
}

