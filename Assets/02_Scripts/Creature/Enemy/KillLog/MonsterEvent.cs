using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonsterEvents
{
    public static event Action<int> OnMonsterKilled;

    public static void RaiseKill(int ID)
    {
        OnMonsterKilled?.Invoke(ID);
    }
}