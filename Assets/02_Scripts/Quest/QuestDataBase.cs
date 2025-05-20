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
        
        Debug.LogWarning($"퀘스트 ID {questId}를 찾을 수 없습니다.");
        return null;
    }
}
