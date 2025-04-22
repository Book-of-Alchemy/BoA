using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerStats PlayerTransform { get; private set; }
    public List<EnemyStats> Enemies { get; private set; } = new List<EnemyStats>();

    public void RegisterPlayer(PlayerStats player)
    {
        PlayerTransform = player;
    }

    public void RegisterEnemy(EnemyStats enemy)
    {
        if (!Enemies.Contains(enemy))
            Enemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyStats enemy)
    {
        if (Enemies.Contains(enemy))
            Enemies.Remove(enemy);
    }

}
