public class DungeonScene : SceneBase
{
    public override SceneType SceneType => SceneType.Dungeon;

    protected override void Initialize()
    {
        // 기존 Awake() 안에 있던 초기화 코드
        UIManager.Instance.RefreshUIList();
    }

    public override void OnEnter()
    {
        UIManager.Show<UI_HUD>();
    }

    public override void OnExit()
    {
        // 씬 종료 로직
    }
}
