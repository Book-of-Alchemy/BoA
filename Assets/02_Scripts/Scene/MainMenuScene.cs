public class MainMenuScene : SceneBase
{
    public override SceneType SceneType => SceneType.MainMenu;

    protected override void Initialize()
    {
        // 기존 Awake() 안에 있던 초기화 코드
    }

    public override void OnEnter()
    {
        UIManager.Show<UI_Main>();
    }

    public override void OnExit()
    {
        // 씬 종료 로직
    }
}
