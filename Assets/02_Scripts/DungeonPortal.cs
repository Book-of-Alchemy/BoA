using UnityEngine;

public class DungeonPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && HasAcceptedQuest())
        {
            EnterDungeon();
        }
        else if (other.CompareTag("Player") && !HasAcceptedQuest())
        {
            UIManager.ShowOnce<UI_Text>("먼저 퀘스트를 수락해야 합니다");
        }
    }

    private bool HasAcceptedQuest()
    {
        return QuestManager.Instance.GetAcceptedQuest() != null;
    }

    private void EnterDungeon()
    {  
        GameSceneManager.Instance.ChangeScene(SceneType.Dungeon);
    }
} 