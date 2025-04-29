using UnityEngine;

public abstract class SceneBase : MonoBehaviour
{
    [SerializeField] private SceneType _sceneType;   // 인스펙터에서 지정할 씬 타입
    public SceneType SceneType => _sceneType;        // 외부에서 읽기 전용

    protected virtual void Awake()
    {
        GameSceneManager.Instance?.RegisterSceneBase(this);
    }

    public virtual void OnExit() { }
    public virtual void OnEnter() { }
}
