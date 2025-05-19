using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudTile : TileEffect, IGround, IWater
{
    public override EnvironmentType EnvType => EnvironmentType.Mud;
    public override void PerformAction()
    {
        StatusEffectFactory.CreateEffect(220006, CurTile.CharacterStatsOnTile);
    }
}
