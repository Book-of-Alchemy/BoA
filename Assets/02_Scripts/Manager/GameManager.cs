using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // 씬 타입 변경 이벤트
    public event Action<SceneType> OnSceneTypeChanged;

    public PlayerStats PlayerTransform { get; private set; }
    public List<EnemyStats> Enemies { get; private set; } = new List<EnemyStats>();

    protected override void Awake()
    {
        base.Awake();
        // gameSceneManager의 이벤트 구독
        GameSceneManager.Instance.OnSceneTypeChanged += sceneType =>
        {
            OnSceneTypeChanged?.Invoke(sceneType);
        };
    }

    private void OnDestroy()
    {
        //구독 해제
        if (GameSceneManager.HasInstance)
            GameSceneManager.Instance.OnSceneTypeChanged -= sceneType =>
            {
                OnSceneTypeChanged?.Invoke(sceneType);
            };
    }

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
