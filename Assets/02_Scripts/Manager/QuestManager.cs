using System;
using System.Collections.Generic;

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

    public bool CanAcceptQuest()
    {
        bool result = AcceptedQuest == null ? true : false;
        return result;
    }

    public void AcceptQuest(QuestData quest)
    {
        if (AcceptedQuest == null)
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
    }

    public void UpdateProgress(int value)
    {
        if (AcceptedQuest == null) return;

        AcceptedQuest.UpdateProgress(value);

        if (AcceptedQuest.ProgressVal > AcceptedQuest.CompleteVal)
            QuestCleared();
    }

    private void QuestCleared()
    {
        //퀘스트 클리어시 초기화
        if (AcceptedQuest == null) return;

        AcceptedQuest.ClearQuest();
        _clearedQuests.Add(AcceptedQuest.Data);
        AcceptedQuest = null;
        OnQuestAccepted = null;
        MonsterEvents.OnMonsterKilled -= CheckBoss;
        TileManger.OnGetDown -= UpdateProgress;
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

    public void CheckBoss(int ID)
    {
        EnemyData enemy = SODataManager.Instance.enemyDataBase.GetEnemyById(ID);
        if (enemy.isBoss)
        {
            UpdateProgress(1);
        }
    }
}