using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolifiedLavaTile : TileEffect, IGround, IWater
{
    public override EnvironmentType EnvType => EnvironmentType.Solidfied_Lava;
    public override void PerformAction()
    {
    }
}

