using TMPro;
using UnityEngine;

public class UI_Quest : MonoBehaviour
{
    [Header("Inspector참조")]
    [SerializeField] private TextMeshProUGUI _questNameTxt;
    [SerializeField] private TextMeshProUGUI _mainGoalTxt;
    [SerializeField] private TextMeshProUGUI _reward01Txt;
    [SerializeField] private TextMeshProUGUI _reward02Txt;
    [SerializeField] private TextMeshProUGUI _reward03Txt;

    [SerializeField] private TextMeshProUGUI _biomeTxt;
    [SerializeField] private TextMeshProUGUI _descTxt;

    private QuestProgress _currentQuest;

    private void OnEnable()
    {
        _currentQuest = QuestManager.Instance.AcceptedQuest;
        UpdateQuestInfo(_currentQuest.Data);
    }

    private void UpdateQuestInfo(QuestData quest)
    {
        _questNameTxt.text = $"퀘스트 : {quest.quest_name_kr}";
        _mainGoalTxt.text = $"목표 : {quest.main_object_text_kr}";
        _reward01Txt.text = $" 보상 : {quest.reward1.ToString()}";
        _reward02Txt.text = $" 보상 : {quest.reward2.ToString()}";
        _reward03Txt.text = $" 보상 : {quest.reward3.ToString()}";
        //바이옴 데이터 없음.
        _biomeTxt.text = $"수행 지역 : 숲";
        _descTxt.text = quest.descriptiontxt;
    }


}
