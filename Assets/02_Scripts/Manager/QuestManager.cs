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
    private List<QuestData> _clearedQuests = new List<QuestData>();
    public QuestProgress GetAcceptedQuest() => _acceptedQuest;

    public bool CanAcceptQuest()
    {
        bool result = _acceptedQuest == null ? true : false;
        return result;
    }

    public void AcceptQuest(QuestData quest)
    {
        if (_acceptedQuest.Data == null)
                _acceptedQuest = new QuestProgress(quest);
    }

    public void UpdateProgress(int value)
    {
        if (_acceptedQuest == null) return;

        _acceptedQuest.UpdateProgress(value);

        if (_acceptedQuest.ProgressVal > _acceptedQuest.CompleteVal)
            QuestCleared();
    }

    private void QuestCleared()
    {
        if (_acceptedQuest == null) return;

        _acceptedQuest.ClearQuest();
        _clearedQuests.Add(_acceptedQuest.Data);
        _acceptedQuest = null;
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
}