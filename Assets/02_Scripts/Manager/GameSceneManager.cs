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

    private void RegisterSceneBase(SceneBase sceneBase)//딕셔너리에 저장하는 메서드
    {
        _sceneMap[sceneBase.SceneType] = sceneBase;
    }

    public void SetSceneType(SceneType newType)//씬 갱신 메서드
    {
        _currentScene = newType;
        OnSceneTypeChanged?.Invoke(_currentScene);
    }

    public void ChangeScene(SceneType nextScene)//씬 전환 메서드(비동기 로딩 시작)
    {
        _currentScene = nextScene;
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()//비동기 로딩
    {
        _currentSceneBase?.OnExit();// 이전 씬의 정리

        //다음 씬 비동기 로딩, 자동 전환 잠시 막기
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_currentScene.ToString());
        loadOperation.allowSceneActivation = false;

        //90프로 완료될때까지 매 프레임 대기
        while (loadOperation.progress < 0.9f)
            yield return null;

        //로딩 최종 승인
        loadOperation.allowSceneActivation = true;
        yield return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!Enum.TryParse(scene.name, out SceneType parsedType))// 씬이름을 통해 정의해둔 타입으로 파싱
        {
            Debug.LogWarning($"Cannot parse scene name: {scene.name}");
            return;
        }

        _currentScene = parsedType;// 새 씬 타입 저장
        OnSceneTypeChanged?.Invoke(_currentScene);//변경사항 알리기

        //해당 씬의 씬베이스를 실행
        if (_sceneMap.TryGetValue(_currentScene, out SceneBase sceneBase))
        {
            _currentSceneBase = sceneBase;
            sceneBase.OnEnter();
        }
        else
        {
            Debug.LogWarning($"다음씬은 매핑되지 않았습니다. {_currentScene}");
        }
    }

    private void OnDestroy()// 이벤트 구독해제
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
