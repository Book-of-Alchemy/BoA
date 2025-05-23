using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QuestDataBase")]
public class QuestDataBase : ScriptableObject
{
    public List<QuestData> questData;

    public QuestData GetQuestData(int questId)
    {
        foreach (QuestData quest in questData)
        {
            if (quest.id == questId)
            {
                return quest;
            }
        }
        
        return null;
    }
}
