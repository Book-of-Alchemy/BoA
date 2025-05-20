using UnityEngine;
using System.IO;

public class TownScene : SceneBase
{
    public override SceneType SceneType => SceneType.Town;

    protected override void Initialize()
    {
        // DataManager 초기화 확인
        if (DataManager.Instance != null)
        {
            Debug.Log("데이터 매니저가 준비되었습니다.");
            Debug.Log($"현재 보유 골드: {DataManager.Instance.GetPlayerData().Gold}");
        }
    }

    public override void OnEnter()
    {
        // 씬 진입 시 데이터 로드
        DataManager.Instance.LoadData();
        Debug.Log("타운 씬에 진입했습니다.");
    }

    public override void OnExit()
    {
        // 씬 종료 시 데이터 저장
        DataManager.Instance.SaveData();
        Debug.Log("타운 씬에서 나갑니다.");
    }

    
}
