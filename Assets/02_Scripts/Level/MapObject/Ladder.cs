using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MapObject
{
    
    public override void Interact()
    {
        TileManger.Instance.GetDownToNextLevel();
    }
}
