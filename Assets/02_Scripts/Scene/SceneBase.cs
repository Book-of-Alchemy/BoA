public abstract class SceneBase
{
    public abstract SceneType SceneType { get; }

    protected SceneBase()//외부에서 new SceneBase() 못하게 막기위한 의도
    {
        Initialize();//생성되는 시점에 공통 초기화하기 위한 훅 메서드
                     //Initialize()를 override해두면 인스턴스가 만들어지는 즉시 구현이 실행됨
    }


    protected virtual void Initialize() { }//생성자 내부에서 자동으로 호출, 기존에 Awake() 에 들어있던, 씬 오브젝트가 생성될 때 필요한 설정들을 이곳으로

    public virtual void OnExit() { }
    public virtual void OnEnter() { }
}
