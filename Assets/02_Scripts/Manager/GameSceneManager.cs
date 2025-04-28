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

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    private SceneType _currentScene;
    private SceneBase _currentSceneBase;

    // 씬 타입 → SceneBase 매핑
    private readonly Dictionary<SceneType, SceneBase> _sceneMap = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// SceneBase.Awake()에서 자기 자신을 등록하기 위해 호출됩니다.
    /// </summary>
    public void RegisterSceneBase(SceneBase sb)
    {
        // 중복 추가 방지
        if (!_sceneMap.ContainsKey(sb.SceneType))
            _sceneMap.Add(sb.SceneType, sb);
    }

    /// <summary>씬 전환 요청</summary>
    public void ChangeScene(SceneType next)
    {
        _currentScene = next;
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        // 1) 이전 씬 종료
        _currentSceneBase?.OnExit();

        // 2) 비동기 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(_currentScene.ToString());
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;
        yield return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 3) 로드된 씬에 대응하는 SceneBase를 딕셔너리에서 꺼내기
        if (_sceneMap.TryGetValue(_currentScene, out var sb))
        {
            _currentSceneBase = sb;
            sb.OnEnter();
        }
        else
        {
            Debug.LogWarning($"[{scene.name}] 씬에 매핑된 SceneBase({_currentScene})가 없습니다.");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

