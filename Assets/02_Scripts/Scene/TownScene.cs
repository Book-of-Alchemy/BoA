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

    // 예시: 골드 추가 메서드
    public void AddGold(int amount)
    {
        DataManager.Instance.AddGold(amount);
        Debug.Log($"{amount} 골드를 획득했습니다. 현재 골드: {DataManager.Instance.GetPlayerData().Gold}");
    }

    // 예시: 퀘스트 수락 메서드
    public void AcceptQuest(int questId)
    {
        DataManager.Instance.AcceptQuest(questId);
        Debug.Log($"퀘스트 ID {questId}를 수락했습니다.");
    }

    // 예시: 퀘스트 완료 메서드
    public void CompleteQuest(int questId)
    {
        DataManager.Instance.CompleteQuest(questId);
        Debug.Log($"퀘스트 ID {questId}를 완료했습니다.");
    }
}
