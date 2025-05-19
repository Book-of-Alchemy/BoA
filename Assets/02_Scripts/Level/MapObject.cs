using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : MonoBehaviour
{
    public MapObjectData data;
    public SpriteRenderer spriteRenderer;
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



    public virtual void Init()
    {
        CurTile.onIsOnSightChanged += UpdateTileVisual;
    }
    public abstract void Interact();

    protected virtual void OnDisable()
    {
        CurTile.onIsOnSightChanged -= UpdateTileVisual;
    }
    public virtual void UpdateTileVisual()
    {
        if (!spriteRenderer) return;

        Color color;
        bool shouldEnable = true;

        if (CurTile.IsOnSight)
        {
            color = Color.white;
        }
        else if (CurTile.IsExplored)
        {
            color = new Color(0.4f, 0.4f, 0.4f);
        }
        else
        {
            shouldEnable = false;
            color = Color.clear;
        }

        spriteRenderer.enabled = shouldEnable;
        spriteRenderer.color = color;
    }
}
