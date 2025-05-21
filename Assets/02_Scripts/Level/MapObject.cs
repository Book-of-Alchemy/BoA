using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : MonoBehaviour
{
    public MapObjectData data;
    protected SpriteRenderer[] spriteRenderers;
    public virtual bool IsOccuPying => false;
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

    protected void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public virtual void Init()
    {
        if (CurTile == null) return;
        CurTile.onIsOnSightChanged += UpdateTileVisual;
        UpdateTileVisual();
        foreach (var renderer in spriteRenderers)
        {
            renderer.sortingOrder = -10 * CurTile.gridPosition.y;
        }
        transform.SetParent(CurTile.curLevel.transform);
    }
    public abstract void Interact();

    protected virtual void OnDisable()
    {
        if (CurTile == null) return;
        CurTile.onIsOnSightChanged -= UpdateTileVisual;
    }
    public virtual void UpdateTileVisual()
    {
        if (spriteRenderers == null) return;

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

        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = shouldEnable;
            renderer.color = color;
        }
        
    }
}
