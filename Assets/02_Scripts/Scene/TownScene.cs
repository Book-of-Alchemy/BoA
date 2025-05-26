using UnityEngine;

public class TownScene : SceneBase
{
    public override SceneType SceneType => SceneType.Town;

    protected override void Initialize()
    {
        // DataManager 초기화 확인
        if (DataManager.Instance != null)
        {
        }
    }

    public override void OnEnter()
    {
        // 씬 진입 시 데이터 로드
        DataManager.Instance.LoadData();
    }

    public override void OnExit()
    {
        // 씬 종료 시 데이터 저장
        DataManager.Instance.SaveData();
    }

    
}
