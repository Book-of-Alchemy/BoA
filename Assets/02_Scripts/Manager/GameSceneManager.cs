using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    MainMenu,
    Dungeon,
    Town,
    Loading
}

public class GameSceneManager : Singleton<GameSceneManager>
{
    public event Action<SceneType> OnSceneTypeChanged;

    private SceneType _currentScene;
    private SceneBase _currentSceneBase;
    private readonly Dictionary<SceneType, SceneBase> _sceneMap = new();
    private SceneType _targetSceneType; // 로딩 후 전환될 씬 타입
    
    // 씬 로딩 처리 중인지 확인하는 플래그
    private bool _isProcessingSceneLoad = false;

    public SceneType CurrentSceneType => _currentScene;

    protected override void Awake()
    {
        base.Awake();

        RegisterSceneBase(new MainMenuScene());
        RegisterSceneBase(new DungeonScene());
        RegisterSceneBase(new TownScene());
        RegisterSceneBase(new LoadingScene());

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

    public void ChangeScene(SceneType nextScene)
    {
        _targetSceneType = nextScene; // 목표 씬 저장
        _currentScene = SceneType.Loading; // 로딩 씬으로 전환
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        _currentSceneBase?.OnExit();// 이전 씬의 정리

        // 로딩 씬 비동기 로딩, 자동 전환
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(SceneType.Loading.ToString());
        while (!loadingOperation.isDone)
        {
            yield return null;
        }

        // 로딩 UI가 나타나면 목표 씬 비동기 로딩 시작
        yield return StartCoroutine(LoadTargetScene());
    }

    private IEnumerator LoadTargetScene()
    {
        // 실제 목표 씬 비동기 로딩
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_targetSceneType.ToString());
        loadOperation.allowSceneActivation = false;

        float minLoadTime = 2.0f; // 최소 로딩 시간(초)
        float timer = 0f;
        
        while (timer < minLoadTime)
        {
            timer += Time.deltaTime;
            
            float timeProgress = Mathf.Clamp01(timer / minLoadTime);
            
            // 실제 로딩 진행률
            float realProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            
            // 두 진행률 중 더 큰 값을 사용
            float finalProgress = Mathf.Min(timeProgress, realProgress);
            
            // UI 업데이트
            LoadingUI.Instance?.UpdateProgress(finalProgress);
            
            // 로딩이 90 이상이고 타이머가 최소 시간의 80를 넘었다면
            if (loadOperation.progress >= 0.9f && timer >= minLoadTime * 0.8f)
            {
                // 남은 시간 계산
                float remainTime = minLoadTime - timer;
                if (remainTime > 0)
                {
                    yield return new WaitForSeconds(remainTime);
                }
                
                loadOperation.allowSceneActivation = true;
                break;
            }
            
            yield return null;
        }
        
        // 로딩이 완료되지 않았다면 활성화
        if (!loadOperation.isDone)
        {
            loadOperation.allowSceneActivation = true;
        }
        
        yield return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.LogWarning("Called");
        
        // 이미 처리 중이면 중복 호출 방지
        if (_isProcessingSceneLoad) 
        {
            return;
        }
        
        _isProcessingSceneLoad = true;
        
        if (!Enum.TryParse(scene.name, out SceneType parsedType))// 씬이름을 통해 정의해둔 타입으로 파싱
        {
            Debug.LogWarning($"Cannot parse scene name: {scene.name}");
            _isProcessingSceneLoad = false;
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
        
        _isProcessingSceneLoad = false;
    }

    private void OnDestroy()// 이벤트 구독해제
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
