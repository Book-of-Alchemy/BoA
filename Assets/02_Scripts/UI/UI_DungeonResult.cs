using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_DungeonResult : UIBase
{
    [SerializeField] private TextMeshProUGUI _locationTxt;
    [SerializeField] private TextMeshProUGUI _goldTxt;
    [SerializeField] private TextMeshProUGUI _itemTxt; 
    [SerializeField] private TextMeshProUGUI _resultTxt;

    private float _goldIncreaseDuration = 1.5f;

    private int _finalGold = 0;
    private int _currentGold = 0;
    private bool _itemLoaded = false;
    private bool _endGainGold= false;

    private Coroutine _goldCoroutine;
    public override bool IsClosable => false;

    public override void Opened(params object[] param)
    {
        InitResult();
    }

    private void InitResult()
    {
        SetResultText();
        _currentGold = Inventory.Instance.Gold;

        //텍스트 초기화
        _locationTxt.text = $"{TileManger.Instance.curLevelIndex}층 : 숲";

        //획득 아이템 텍스트 출력
        _itemTxt.text = string.Empty;
        List<InventoryItem> obtainedItems = Inventory.Instance.GetAllItems();
        foreach (var item in obtainedItems)
        {
            if (!item.IsEmpty)
            {
                _itemTxt.text += $"{item.itemData.name_kr} x {item.Amount}\n";
                _finalGold += item.itemData.price * item.Amount;
            }
        }
        Inventory.Instance.IncreaseGold(_finalGold);

        _goldTxt.text = FormatGold(_currentGold);

        
        _itemLoaded = true;
        _endGainGold = false;
    }

    private void SetResultText()
    {
        StringBuilder sb = new StringBuilder();

        QuestProgress quest = QuestManager.Instance.GetAcceptedQuest();
        int artifactCnt = GameManager.Instance.PlayerTransform.equipArtifacts.Count;
        
        //결과 이벤트 변경시 추가
        if (quest != null)
            sb.AppendLine(quest.IsClear ? $"{quest.Data.quest_name_kr} 클리어 " : $"{quest.Data.quest_name_kr} 실패");
        if(artifactCnt > 0)
            sb.AppendLine($"아티팩트 {artifactCnt}개 획득");

        _resultTxt.text = sb.ToString();
    }

    private void Update()
    {
        //아이템 로드가 된 후 입력을 통해 아이템을 골드로 환산
        if (!_itemLoaded) return;

        if (Input.GetKeyDown(KeyCode.Return) && _goldCoroutine == null)
        {
            if (_endGainGold == true)
                HideDirect();

            //인벤토리에서 아이템제거
            _itemTxt.text = string.Empty;
            Inventory.Instance.ClearInventory();
            if (_finalGold > 0)
            {
                _goldCoroutine = StartCoroutine(AnimateGoldGain());
            }
            else
                _endGainGold = true;
        }
    }

    private IEnumerator AnimateGoldGain()
    {
        _finalGold += _currentGold;
        //현재 골드를 아이템 가치 총 합산 골드로 증가시키는 애니메이션
        DOTween.To(() => _currentGold, x =>
        {
            _currentGold = x;
            _goldTxt.text = FormatGold(_currentGold);
        }, _finalGold, _goldIncreaseDuration).SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(_goldIncreaseDuration);

        _goldTxt.text = FormatGold(_finalGold);
        
        _endGainGold = true;
        _goldCoroutine = null;
        yield break;
    }
    private string FormatGold(int amount)
    {
        return $"{amount:N0}G";
    }

    public override void HideDirect()
    {
        UIManager.Hide<UI_DungeonResult>();
    }
}
