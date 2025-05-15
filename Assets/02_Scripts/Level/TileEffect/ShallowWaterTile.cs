using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShallowWaterTile : TileEffect, IGround,IWater
{
    public override EnvironmentType EnvType => EnvironmentType.Shallow_Water;
    public override void PerformAction()
    {

    }
}
