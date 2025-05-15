using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPrefab : TilePrefab
{
    public event Action OnReturnEvent;
    public EnvironmentalData data;
    
    public void OnReturn()
    {
        OnReturnEvent?.Invoke();
    }

    public void AutoTileUpdate()
    {
        var neighbor = TileUtility.GetAdjacentTileList(CurTile.curLevel, CurTile);
        foreach (var tile in neighbor) {

    }
}
