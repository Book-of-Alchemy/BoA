using System;

public static class MonsterEvents
{
    public static event Action<int> OnMonsterKilled;

    public static void RaiseKill(int ID)
    {
        OnMonsterKilled?.Invoke(ID);
    }
}