using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactWindow : MonoBehaviour
{
    [SerializeField] private List<ArtifactSlotUI> _slots;
    [SerializeField] private RectTransform _selector;
    [SerializeField] private Button _nextBtn;
    [SerializeField] private Button _prevBtn;
    [SerializeField] private UIAnimator _animator;
    [SerializeField] private List<RectTransform> _artifactBorder;
    [SerializeField][Range(0.1f,2f)]private float slideDuration = 1f;

    [Header("Info")] //유물 선택시 보여지는 정보
    [SerializeField] private Image _artifactImage;
    [SerializeField] private TextMeshProUGUI _artifactName;
    [SerializeField] private TextMeshProUGUI _artifactDesc;
    [SerializeField] private TextMeshProUGUI _artifactRarity;

    //활성화 될때 갱신용 Artifact데이터들
    private List<ArtifactData> _allArtifacts = new();

    private int _currentPage = 0;
    private const int PageSize = 16;

    private Color _commonColor = Color.white;
    private Color _unCommonColor = Color.green;
    private Color _rareColor = Color.blue;

    private void OnEnable()
    {
        _currentPage = 0;

        List<Artifact> data = GameManager.Instance.PlayerTransform.equipArtifacts;
        //플레이어 장착된 아티팩트 전부 불러오기
        foreach (var item in data)
            AddArtifact(item.data);

        foreach (var item in _slots)
            item.OnBtnSelected += MoveSelectorTo;

        UpdateSlotUI();
    }

    private void OnDisable()
    {
        _allArtifacts.Clear();
    }

    //선택 표시기를 이동시키는 메서드
    private void MoveSelectorTo(RectTransform target,ArtifactData data)
    {
        Vector3 basePos = target.position;
        _selector.DOMove(basePos, 0f);
        if (data != null)
            UpdateInfo(data);
        else
            ClearInfo();
    }

    public void AddArtifact(ArtifactData data)
    {
        _allArtifacts.Add(data);
    }

    public void NextPage()
    {
        //16배수로 Page를 늘리게 됨.
        int maxPage = Mathf.CeilToInt(_allArtifacts.Count / (float)PageSize) - 1;
        if (_currentPage < maxPage)
        {
            _currentPage++;
            HideBorderToUpdate();
        }
    }

    public void PrevPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            HideBorderToUpdate();
        }
    }

    private void UpdateSlotUI()
    {
        int startIndex = _currentPage * PageSize;
        for (int i = 0; i < PageSize; i++)
        {
            int Index = startIndex + i;
            if (Index < _allArtifacts.Count)
            {
                _slots[i].SetData(_allArtifacts[Index]);
            }
            else
                _slots[i].RemoveData();
        }
        SelectFirst();
    }

    private void SelectFirst() //slots의 첫번재 인덱스를 선택하게함.
    {
        _slots[0].OnSelect(null);
        _slots[0]._btn.Select();
    }

    private void HideBorderToUpdate()
    {
        int Count = 0;
        int total = _artifactBorder.Count;
        Vector2[] originalPositions = new Vector2[total];

        // 원래 위치 저장
        for (int i = 0; i < total; i++)
            originalPositions[i] = _artifactBorder[i].anchoredPosition;

        _selector.gameObject.SetActive(false);

        // 이동 애니메이션
        for (int i = 0; i < total; i++)
        {
            float targetX = (i % 2 == 0) ? -500f : 500f;

            _artifactBorder[i].DOAnchorPosX(targetX, slideDuration, true)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                Count++;
                if (Count == total)
                {
                    UpdateSlotUI();

                    // 모든 도착이 완료되면 복귀 시작
                    ReturnAllBorders(originalPositions);
                }
            });
        }
    }

    void ReturnAllBorders(Vector2[] originalPositions)
    {
        int total = _artifactBorder.Count;
        //Border들이 UI업데이트 되었고 originalPositions으로 복귀
        for (int i = 0; i < total; i++)
        {
            int index = i;
            Sequence moveSeq = DOTween.Sequence();
            moveSeq.Append(_artifactBorder[index]
                .DOAnchorPosX(originalPositions[index].x, slideDuration, true)
                .SetEase(Ease.InOutSine));
            moveSeq.OnComplete(() =>
            {
                //Selector활성화 및 첫번째 Slot을 선택
                _selector.gameObject.SetActive(true);  
                SelectFirst();
            });
        }
    }
    private void UpdateInfo(ArtifactData data)
    {
        _artifactImage.sprite = data.icon_sprite;
        _artifactName.text = data.name_kr;
        _artifactDesc.text = data.description;
        _artifactRarity.text = data.rarity.ToString();

        _artifactRarity.color = data.rarity switch
        {
            Rarity.Common => _commonColor,
            Rarity.Uncommon => _unCommonColor,
            Rarity.Rare => _rareColor,
            _ => _commonColor // 기본값
        };
    }

    private void ClearInfo()
    {
        _artifactImage.sprite = null;
        _artifactName.text = string.Empty;
        _artifactDesc.text = string.Empty;
        _artifactRarity.text = string.Empty;
    }
}
