public abstract class SceneBase
{
    public abstract SceneType SceneType { get; }

    protected SceneBase()
    {
        Initialize();
    }

 
    protected virtual void Initialize() { }

    public virtual void OnExit() { }
    public virtual void OnEnter() { }
}
