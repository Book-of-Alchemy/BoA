using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MapObject
{

    public override void Init()
    {
        CurTile.onCharacterChanged += Interact;
    }
    public override void Interact()
    {
        if (CurTile.CharacterStatsOnTile is PlayerStats)
            TileManger.Instance.GetDownToNextLevel();
    }

    private void OnDisable()
    {
        CurTile.onCharacterChanged -= Interact;
    }
}
