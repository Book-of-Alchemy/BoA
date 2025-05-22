public class DungeonScene : SceneBase
{
    public override SceneType SceneType => SceneType.Dungeon;

    protected override void Initialize()
    {
        // 기존 Awake() 안에 있던 초기화 코드
        
    }

    public override void OnEnter()
    {
        UIManager.Instance.RefreshUIList();
        // UI 확인 전에 UIManager가 준비되었는지 확인
        if (!UIManager.IsOpened<UI_HUD>())
        {
            UIManager.Show<UI_HUD>();
        }
        TurnManager.Instance.ResetManager();
        TileManger.Instance.GenerateDungeon();
        TurnManager.Instance.StartTurnCycle();
    }

    public override void OnExit()
    {
        // 씬 종료 로직
    }
}
