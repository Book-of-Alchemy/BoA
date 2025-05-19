using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MapObject
{

    public override void Init()
    {
        base.Init();
        CurTile.onCharacterChanged += Interact;
    }
    public override void Interact()
    {
        if (CurTile.CharacterStatsOnTile is PlayerStats)
            TileManger.Instance.GetDownToNextLevel();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CurTile.onCharacterChanged -= Interact;
    }
}
