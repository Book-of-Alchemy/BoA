using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : MonoBehaviour
{
    public MapObjectData data;
    protected Tile curTile;
    public Tile CurTile
    {
        get => curTile; 
        set 
        { 
            curTile = value;
            Init();
        }
    }



    public abstract void Init();
    public abstract void Interact();
}
