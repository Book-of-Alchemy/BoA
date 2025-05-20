using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetail : MonoBehaviour
{
    [Header("Information")]
    [SerializeField] private TextMeshProUGUI _questName;
    [SerializeField] private TextMeshProUGUI _mainObjective;
    [SerializeField] private TextMeshProUGUI _rewardGold;
    [SerializeField] private TextMeshProUGUI _reward1;
    [SerializeField] private TextMeshProUGUI _reward2;
    [SerializeField] private TextMeshProUGUI _reward3;
    [SerializeField] private TextMeshProUGUI _client;
    [SerializeField] private TextMeshProUGUI _description;

    private QuestData _currentQuest;
    public void ShowDetail(QuestData quest)
    {
        _currentQuest = quest;

        _questName.text = quest.quest_name_kr;
        _mainObjective.text = $"주 목표 : {quest.main_object_text_kr} ";
        _rewardGold.text = $"골드 보상 : {quest.reward_gold_amount} G";
        _reward1.text = quest.reward1 != Reward.none ? $"보상1 : {quest.reward1} " : string.Empty;
        _reward2.text = quest.reward2 != Reward.none ? $"보상2 : {quest.reward1} " : string.Empty;
        _reward3.text = quest.reward3 != Reward.none ? $"보상3 : {quest.reward1} " : string.Empty;
        _client.text = $"의뢰인 : {quest.client}";
        _description.text = $"내용 : {quest.descriptiontxt}";
    }

    public void OnAccept()
    {
        //퀘스트 수락
        QuestManager.Instance.AcceptQuest(_currentQuest);
        UIManager.Hide<UI_SelectQuest>();
    }

    public void OnDecline()
    {
        //이전화면
        gameObject.SetActive(false);
    }
}
