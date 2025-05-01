using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType { MainMenu, Dungeon, Town }

public class GameSceneManager : Singleton<GameSceneManager>
{
    public event Action<SceneType> OnSceneTypeChanged;
    private SceneType _currentScene;
    public SceneType CurrentSceneType => _currentScene;
    private SceneBase _currentSceneBase;
    private readonly Dictionary<SceneType, SceneBase> _sceneMap = new();

    protected override void Awake()
    {
        base.Awake();
        //RegisterSceneBase(new MainMenuScene());
        RegisterSceneBase(new DungeonScene());
        //RegisterSceneBase(new TownScene());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void RegisterSceneBase(SceneBase sb) => _sceneMap[sb.SceneType] = sb;
    public void SetSceneType(SceneType newType) { _currentScene = newType; OnSceneTypeChanged?.Invoke(_currentScene); }
    public void ChangeScene(SceneType next) { _currentScene = next; StartCoroutine(LoadRoutine()); }
    private IEnumerator LoadRoutine()
    {
        _currentSceneBase?.OnExit();
        var op = SceneManager.LoadSceneAsync(_currentScene.ToString()); op.allowSceneActivation = false;
        while (op.progress < 0.9f) yield return null;
        op.allowSceneActivation = true; yield return null;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!Enum.TryParse(scene.name, out SceneType type)) { Debug.LogWarning($"Cannot parse {scene.name}"); return; }
        _currentScene = type; OnSceneTypeChanged?.Invoke(_currentScene);
        if (_sceneMap.TryGetValue(_currentScene, out var sb)) { _currentSceneBase = sb; sb.OnEnter(); }
        else Debug.LogWarning($"No SceneBase for {_currentScene}");
    }
    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;
}
