using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestProgress
{
    public QuestData Data { get; private set; }
    public bool IsClear { get; private set; }
    public int ProgressVal { get; private set; }
    public int CompleteVal { get; private set; }

    public QuestProgress(QuestData data)
    {
        Data = data;
        IsClear = false;
        ProgressVal = 0;
        //ActiveEvent가 변경사항이 있을 수 있음.
        //퀘스트 달성 조건은 진행도가 FloorCount에 도달하는것 
        if (data.main_object_type == ObjectType.ActivateEvent)
            CompleteVal = data.dungeon_floor_count;
        else
            CompleteVal = 1;
    }

    public void UpdateProgress(int value)
    {
        ProgressVal = value;
    }

    public void ClearQuest()
    {
        IsClear = true;
    }
}

public class QuestManager : Singleton<QuestManager>
{
    private QuestProgress _acceptedQuest;

    public QuestProgress AcceptedQuest
    {
        get { return _acceptedQuest; }
        set
        {
            _acceptedQuest = value;
            OnQuestAccepted?.Invoke();
        }
    }

    private List<QuestData> _clearedQuests = new List<QuestData>();
    public QuestProgress GetAcceptedQuest() => _acceptedQuest;

    public event Action OnQuestAccepted;

    protected override void Awake()
    {
        base.Awake();
        LoadSavedQuestData();
    }

    private void LoadSavedQuestData()
    {
        // DataManager로부터 저장된 퀘스트 정보 로드
        if (DataManager.Instance != null)
        {
            PlayerData playerData = DataManager.Instance.GetPlayerData();
            
            // 이미 완료한 퀘스트 목록 복원
            List<int> clearedQuestIds = playerData.ClearedQuests;
            foreach (int questId in clearedQuestIds)
            {
                QuestData questData = FindQuestDataById(questId);
                if (questData != null)
                {
                    _clearedQuests.Add(questData);
                    Debug.Log($"완료한 퀘스트 복원: ID {questId} - {questData.quest_name_kr}");
                }
            }
            
            // 현재 수락 중인 퀘스트 복원
            List<int> acceptedQuestIds = playerData.AcceptedQuests;
            if (acceptedQuestIds.Count > 0)
            {
                // 현재 구조에서는 하나의 퀘스트만 활성화할 수 있으므로 첫 번째 것만 가져옴
                int acceptedQuestId = acceptedQuestIds[0];
                QuestData questData = FindQuestDataById(acceptedQuestId);
                if (questData != null)
                {
                    AcceptedQuest = new QuestProgress(questData);
                    Debug.Log($"수락 중인 퀘스트 복원: ID {acceptedQuestId} - {questData.quest_name_kr}");
                }
            }
        }
    }

    // ID로 QuestData를 찾는 메서드
    private QuestData FindQuestDataById(int questId)
    {
        // 퀘스트 데이터베이스에서 ID로 퀘스트 찾기
        QuestDataBase questDataBase = SODataManager.Instance.GetQuestDataBase();
        if (questDataBase != null)
        {
            return questDataBase.GetQuestData(questId);
        }
        
        return null;
    }

    public bool CanAcceptQuest()
    {
        bool result = AcceptedQuest == null ? true : false;
        return result;
    }

    public void AcceptQuest(QuestData quest)
    {
        Unsubscribe();
        if (AcceptedQuest == null)
        {
            AcceptedQuest = new QuestProgress(quest);
            switch (quest.main_object_type)
            {
                case ObjectType.DefeatBoss:
                    MonsterEvents.OnMonsterKilled += CheckBoss;
                    break;
                case ObjectType.ReachLastEscape:
                    TileManger.OnGetDown += UpdateProgress;
                    break;
                default:
                    break;
            }

            // DataManager에 퀘스트 수락 정보 저장
            this.AcceptQuest(quest.id);
            Debug.Log($"퀘스트 '{quest.quest_name_kr}' (ID: {quest.id})를 수락하고 저장했습니다.");
        }
    }

    public void UpdateProgress(int value)
    {
        if (AcceptedQuest == null) return;

        AcceptedQuest.UpdateProgress(value);

        if (AcceptedQuest.ProgressVal >= AcceptedQuest.CompleteVal)
            QuestCleared();
    }

    private void QuestCleared()
    {
        //퀘스트 클리어시 초기화
        if (AcceptedQuest == null) return;

        AcceptedQuest.ClearQuest();
        _clearedQuests.Add(AcceptedQuest.Data);
        
        // DataManager에 퀘스트 완료 정보 저장
        this.CompleteQuest(AcceptedQuest.Data.id);
        Debug.Log($"퀘스트 '{AcceptedQuest.Data.quest_name_kr}' (ID: {AcceptedQuest.Data.id})를 완료하고 저장했습니다.");
        
        AcceptedQuest = null;
        OnQuestAccepted = null;
        Unsubscribe();
    }

    public List<int> GetClearedQuestIds()
    {
        List<int> clearedIds = new List<int>();

        for (int i = 0; i < _clearedQuests.Count; i++)
        {
            clearedIds.Add(_clearedQuests[i].id);
        }

        return clearedIds;
    }

    // DataManager에서 가져온 퀘스트 관련 메서드
    public void AcceptQuest(int questId)
    {
        // PlayerData에 퀘스트 ID 저장
        if (DataManager.Instance != null)
        {
            if (!DataManager.Instance.GetPlayerData().AcceptedQuests.Contains(questId) && 
                !DataManager.Instance.GetPlayerData().ClearedQuests.Contains(questId))
            {
                DataManager.Instance.GetPlayerData().AcceptedQuests.Add(questId);
                DataManager.Instance.SaveData();
            }
        }
    }

    public void CompleteQuest(int questId)
    {
        // PlayerData에서 수락한 퀘스트 목록에서 제거하고 완료한 퀘스트 목록에 추가
        if (DataManager.Instance != null)
        {
            if (DataManager.Instance.GetPlayerData().AcceptedQuests.Contains(questId))
            {
                DataManager.Instance.GetPlayerData().AcceptedQuests.Remove(questId);
                DataManager.Instance.GetPlayerData().ClearedQuests.Add(questId);
                DataManager.Instance.SaveData();
            }
        }
    }

    public void CheckBoss(int ID)
    {
        EnemyData enemy = SODataManager.Instance.enemyDataBase.GetEnemyById(ID);
        if (enemy == null) return;
        if (enemy.isBoss)
            UpdateProgress(1);
    }

    void Unsubscribe()
    {
        MonsterEvents.OnMonsterKilled -= CheckBoss;
        TileManger.OnGetDown -= UpdateProgress;
    }
}