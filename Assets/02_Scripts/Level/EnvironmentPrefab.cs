using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class EnvironmentPrefab : TilePrefab
{
    public event Action OnReturnEvent;
    public EnvironmentalData data;
    public int framePerSec = 12;
    public float frameDuration => 1f / framePerSec;
    protected Sequence animationSequence;
    protected Material originMatrial;
    public void Initiallize(EnvironmentalData data, Tile tile)
    {
        this.data = data;
        CurTile = tile;
        originMatrial = baseRenderer.material;
        if (data.material != null)
            baseRenderer.material = data.material;
    }
    public void OnReturn()
    {
        OnReturnEvent?.Invoke();
        baseRenderer.material = originMatrial;
        StopAnimation();
        data = null;
        CurTile = null;
        OnReturnEvent = null;
    }

    public void AutoTileUpdate()
    {
        AutoTiling();
        var neighbor = TileUtility.GetAdjacentTileList(CurTile.curLevel, CurTile);
        foreach (var tile in neighbor)
        {
            if (tile == null) continue;
            if (tile.groundEffect == null) continue;
            tile.groundEffect.prefab.AutoTiling();
        }
    }

    public void AutoTiling()
    {
        if (data == null || CurTile == null) return;
        if (data.environmentalSprites.Count <= 0) return;

        int bitmask = TilePainter.CalculateEnvironmentBitmaskByTileEffect(CurTile, CurTile.curLevel, data.environment_type);
        baseRenderer.sprite = data.GetSprite(bitmask);
    }

    public void PlayAnimation()
    {
        animationSequence?.Kill();

        if (data == null) return;
        List<Sprite> frames = data.animationSprites;
        if (frames.Count <= 0) return;
        animationSequence = DOTween.Sequence();

        foreach (var frame in frames)
        {
            animationSequence.AppendCallback(() => baseRenderer.sprite = frame)
                              .AppendInterval(frameDuration);
        }

        animationSequence.SetLoops(-1);
    }

    public void StopAnimation()
    {
        animationSequence?.Kill();
    }
}
