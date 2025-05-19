using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPrefab : TilePrefab
{
    public event Action OnReturnEvent;
    
    public void OnReturn()
    {
        OnReturnEvent?.Invoke();
    }
}
