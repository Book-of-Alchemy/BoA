using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    MainMenu,
    Dungeon,
    Town
}

public class GameSceneManager : Singleton<GameSceneManager>
{
    private SceneType _currentScene;
    private SceneBase _currentSceneBase;
    private readonly Dictionary<SceneType, SceneBase> _sceneMap = new();

    protected override void Awake()
    {
        base.Awake();

        // new로 모든 씬 베이스 생성
        //RegisterSceneBase(new MainMenuScene());
        //RegisterSceneBase(new TownScene());
        RegisterSceneBase(new DungeonScene());
        

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void RegisterSceneBase(SceneBase sb)
    {
        _sceneMap[sb.SceneType] = sb;
    }

    /// <summary>씬 전환 요청</summary>
    public void ChangeScene(SceneType next)
    {
        _currentScene = next;
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        _currentSceneBase?.OnExit(); // 이전 씬 종료

        var op = SceneManager.LoadSceneAsync(_currentScene.ToString());
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;
        yield return null; // 한 프레임 대기
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_sceneMap.TryGetValue(_currentScene, out var sb))
        {
            _currentSceneBase = sb;
            sb.OnEnter();
        }
        else
        {
            Debug.LogWarning($"[{scene.name}]에 매핑된 SceneBase({_currentScene})가 없습니다.");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}