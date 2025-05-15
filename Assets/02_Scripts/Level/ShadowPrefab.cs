using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPrefab : MonoBehaviour
{

    [SerializeField]
    private Tile curTile;
    public Tile CurTile
    {
        get => curTile;
        set
        {
            if (curTile != null)
            {
                curTile.onIsExploredChanged -= UpdateTileVisual;
                curTile.onIsOnSightChanged -= UpdateTileVisual;
            }

            curTile = value;

            if (curTile != null)
            {
                curTile.onIsExploredChanged += UpdateTileVisual;
                curTile.onIsOnSightChanged += UpdateTileVisual;
                UpdateTileVisual();
            }
        }
    }
    public SpriteRenderer baseRenderer;
    private void OnDestroy()
    {
        if (curTile != null)
        {
            curTile.onIsExploredChanged -= UpdateTileVisual;
            curTile.onIsOnSightChanged -= UpdateTileVisual;
        }
    }
    public void UpdateTileVisual()
    {
        if (!baseRenderer)
            return;

        if (CurTile.IsOnSight)
        {
            baseRenderer.color = Color.white;
        }
        else if (CurTile.IsExplored)
        {
            baseRenderer.color = new Color(0.4f, 0.4f, 0.4f);
        }
        else
        {
            baseRenderer.color = Color.black;
        }
    }
}
