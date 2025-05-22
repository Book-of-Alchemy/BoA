using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // 씬 타입 변경 이벤트
    public event Action<SceneType> OnSceneTypeChanged;
    public KillTracker killTracker;
    public PlayerStats PlayerTransform { get; private set; }
    //public List<EnemyStats> Enemies { get; private set; } = new List<EnemyStats>();

    protected override void Awake()
    {
        base.Awake();
        killTracker = new KillTracker();
        if (killTracker != null)
            MonsterEvents.OnMonsterKilled += killTracker.ReportKill;
        // gameSceneManager의 이벤트 구독 , 씬이 바뀔때마다 메서드 실행됨
        GameSceneManager.Instance.OnSceneTypeChanged += SceneChange;
    }

    private void OnDestroy()
    {
        //구독 해제
        if (GameSceneManager.HasInstance)
            GameSceneManager.Instance.OnSceneTypeChanged -= SceneChange;
        if (killTracker != null)
            MonsterEvents.OnMonsterKilled -= killTracker.ReportKill;
    }
    public void SceneChange(SceneType sceneType)
    {
        OnSceneTypeChanged?.Invoke(sceneType);
        if (sceneType == SceneType.Dungeon)
            killTracker.ResetDungeonKills();
    }
    public void RegisterPlayer(PlayerStats player)
    {
        PlayerTransform = player;
    }

    //public void RegisterEnemy(EnemyStats enemy)
    //{
    //    if (!Enemies.Contains(enemy))
    //        Enemies.Add(enemy);
    //}

    //public void UnregisterEnemy(EnemyStats enemy)
    //{
    //    if (Enemies.Contains(enemy))
    //        Enemies.Remove(enemy);
    //}
}
