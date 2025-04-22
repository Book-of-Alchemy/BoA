using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : MonoBehaviour
{
    public MapObjectData data;
    public Tile tile;

    protected virtual void Start()
    {
     Init();   
    }

    public abstract void Init();
    public abstract void Interact();
}
