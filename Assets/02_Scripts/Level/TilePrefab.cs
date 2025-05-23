using UnityEngine;

public class TilePrefab : MonoBehaviour
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
    public SpriteRenderer upperRenderer;
    private void OnDisable()
    {
        if (curTile != null)
        {
            curTile.onIsExploredChanged -= UpdateTileVisual;
            curTile.onIsOnSightChanged -= UpdateTileVisual;
        }
    }
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
        if (!baseRenderer) return;

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

        baseRenderer.enabled = shouldEnable;
        baseRenderer.color = color;

        if (upperRenderer != null)
        {
            upperRenderer.enabled = shouldEnable;
            upperRenderer.color = color;
        }
    }
}
