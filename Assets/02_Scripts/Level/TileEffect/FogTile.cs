using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTile : TileEffect, IAir, IExpirable
{
    public override EnvironmentType EnvType => EnvironmentType.Fog;
    private int leftTime = 50;
    public int LeftTime { get => leftTime; set => leftTime = value; }
    public override void PerformAction()
    {
        CurTile.IsOnSight = false;
        if (LeftTime <= 0)
        {
            Expire();
            return;
        }

        LeftTime -= ActionCost;
    }

    public void Expire()
    {
        EnvironmentalFactory.Instance.ReturnTileEffect(this);
    }
}

