using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectQuest : UIBase
{
    [Header("UI 참조")]
    [SerializeField] private List<Button> _biomeButtons; // 바이옴 버튼들
    [SerializeField] private GameObject _biomeObject;
    [SerializeField] private Transform _questListParent; // 퀘스트 버튼 위치
    [SerializeField] private GameObject _questObject; 
    [SerializeField] private GameObject _questButtonPrefab; 
    [SerializeField] private QuestDataBase _questDataBase;
    [SerializeField] private QuestDetail _questDetail;

    //바이옴 버튼, 바이옴 id 인덱스에 따른 매핑필요
    [SerializeField] private List<int> _biomeIds; 

    private List<GameObject> _createdQuestBtns = new();
    private void ShowBiomeBtn() => _biomeObject.SetActive(true);
    private void HideBiomeBtn() => _biomeObject.SetActive(false);
    private void ShowQuestBtn() => _questObject.SetActive(true);
    private void HideQuestBtn() => _questObject.SetActive(false);
    private void ShowQuestDetail() => _questDetail.gameObject.SetActive(true);
    private void HideQuestDetail() => _questDetail.gameObject.SetActive(false);


    private void Start()
    {
        for (int i = 0; i < _biomeButtons.Count; i++)
        {
            int index = i;
            _biomeButtons[i].onClick.AddListener(() => OnSelectBiome(_biomeIds[index]));
        }

        //OnSelectBiome(120001); // 숲 자동 선택
    }
    public override void HideDirect()
    {
        UIManager.Hide<UI_SelectQuest>();
    }

    public override void Opened(params object[] param)
    {
        //이미 퀘스트를 받았다면 리턴
        if (!QuestManager.Instance.CanAcceptQuest())
        {
            UIManager.Show<UI_Text>("퀘스트를 완료하고 다시오자");
            HideDirect();
            return;
        }
        _questDataBase = SODataManager.Instance.questDataBase;
        ShowBiomeBtn();
        HideQuestBtn();
        HideQuestDetail();
    }

    private void OnSelectBiome(int biomeId)
    {
        if (biomeId != 120001)
        {
            UIManager.Show<UI_Text>("아직 안될것 같다.");
            return;
        }

        HideBiomeBtn();
        ShowQuestsForBiome(biomeId);
    }

    private void ShowQuestsForBiome(int biomeId)
    {
        ShowQuestBtn();
        //기존 생성 버튼 제거
        foreach (var go in _createdQuestBtns)
            Destroy(go);
        _createdQuestBtns.Clear();

        //선택된 바이옴과 같은 바이옴 id를 가진 퀘스트를 배열저장
        var matched = _questDataBase.questData
            .Where(q => q.biome_id == biomeId)
            //.Where(q => !QuestManager.Instance.GetAcceptedQuest().Data)
            .Where(q => !QuestManager.Instance.GetClearedQuestIds().Contains(q.id)) //클리어 되지않은 퀘스트만
            .ToList();

        //바이옴 퀘스트 중 main 퀘스트 오름차순으로 가장 낮은 번호 반환저장
        QuestData mainQuest = matched
            .Where(q => q.quest_Type == QuestType.main)
            .OrderBy(q => q.id)
            .FirstOrDefault();

        //메인퀘스트 버튼 생성
        if (mainQuest != null)
            CreateQuestButton(mainQuest);

        //서브 퀘스트 배열 저장
        var subQuests = matched
            .Where(q => q.quest_Type == QuestType.sub)
            .OrderBy (q => q.id)
            .ToList();

        //서브 퀘스트 버튼생성
        for (int i = 0; i < subQuests.Count; i++)
            CreateQuestButton(subQuests[i]);
    }

    private void CreateQuestButton(QuestData quest)
    {
        //퀘스트 버튼 집합 하위로 생성 
        GameObject go = Instantiate(_questButtonPrefab, _questListParent);
        //퀘스트에 맞는 Text로 변경
        var text = go.GetComponentsInChildren<TextMeshProUGUI>();
        text[0].text = quest.name;  
        text[1].text = quest.quest_Type.ToString();  

        //버튼 이벤트 등록
        Button btn = go.GetComponent<Button>();
        btn.onClick.AddListener(() => OnClickQuestBtn(quest));
        
        _createdQuestBtns.Add(go);
    }

    private void OnClickQuestBtn(QuestData quest)
    {
        _questDetail.ShowDetail(quest);
        ShowQuestDetail();
    }
}
