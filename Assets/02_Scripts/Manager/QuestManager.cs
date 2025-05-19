
public class QuestManager : Singleton<QuestManager>
{
    private QuestData _acceptedQuest;

    public QuestData GetAcceptedQuest() => _acceptedQuest;
    public void AcceptQuest(QuestData quest)
    {
        if (_acceptedQuest != quest)
            _acceptedQuest = quest;
    }
    
    public bool CanAcceptQuest()
    {
        bool result = _acceptedQuest == null ? true : false;
        return result;
    }

}