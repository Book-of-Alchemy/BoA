using TMPro;
using UnityEngine;

public class UI_Quest : UIBase
{
    [Header("Inspector참조")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _biomeText;
    [SerializeField] private TextMeshProUGUI _descText;



    [SerializeField] private UIAnimator _uiAnimator;
    [SerializeField] private Transform _questListParent;
    [SerializeField] private GameObject _questListItemPrefab;
    public override void HideDirect()
    {
        
    }

    public override void Opened(params object[] param)
    {
        ClearList();
    }

    private void SetList(QuestData quest)
    {
        _titleText.text = quest.name_kr;
        _biomeText.text = $"바이옴 이름 (ID: {quest.biome_id})";
        _descText.text = GenerateDescription(quest);
    }

    private string GenerateDescription(QuestData quest)
    {
        return $"퀘스트 설명\n몬스터 레벨: {quest.base_monster_level} ~ (층당 +{quest.level_per_floor})\n" +
               $"맵 크기 비율: 소({quest.map_size_prob_small}), 중({quest.map_size_prob_medium}), 대({quest.map_size_prob_large})\n";
    }
    private void ClearList()
    {
        foreach (Transform child in _questListParent)
            Destroy(child.gameObject);
    }
}
