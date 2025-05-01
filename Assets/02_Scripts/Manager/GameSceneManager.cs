using System;
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
    public event Action<SceneType> OnSceneTypeChanged;

    private SceneType _currentScene;
    private SceneBase _currentSceneBase;
    private readonly Dictionary<SceneType, SceneBase> _sceneMap = new();

    public SceneType CurrentSceneType => _currentScene;

    protected override void Awake()
    {
        base.Awake();

        // RegisterSceneBase(new MainMenuScene());
        RegisterSceneBase(new DungeonScene());
        RegisterSceneBase(new TownScene());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void RegisterSceneBase(SceneBase sceneBase)
    {
        _sceneMap[sceneBase.SceneType] = sceneBase;
    }

    public void SetSceneType(SceneType newType)
    {
        _currentScene = newType;
        OnSceneTypeChanged?.Invoke(_currentScene);
    }

    public void ChangeScene(SceneType nextScene)
    {
        _currentScene = nextScene;
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        _currentSceneBase?.OnExit();

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_currentScene.ToString());
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
            yield return null;

        loadOperation.allowSceneActivation = true;
        yield return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!Enum.TryParse(scene.name, out SceneType parsedType))
        {
            Debug.LogWarning($"Cannot parse scene name: {scene.name}");
            return;
        }

        _currentScene = parsedType;
        OnSceneTypeChanged?.Invoke(_currentScene);

        if (_sceneMap.TryGetValue(_currentScene, out SceneBase sceneBase))
        {
            _currentSceneBase = sceneBase;
            sceneBase.OnEnter();
        }
        else
        {
            Debug.LogWarning($"No SceneBase registered for {_currentScene}");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
