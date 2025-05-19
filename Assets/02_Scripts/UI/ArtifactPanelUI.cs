using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactPanelUI : MonoBehaviour, ISelectHandler
{
    [Header("Necessary Fields")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameTxt;
    [SerializeField] private TextMeshProUGUI _descTxt;
    [SerializeField] private TextMeshProUGUI _rarityTxt;
    [SerializeField] private RectTransform _rect;
    public Button _btn;

    private ArtifactData _data;
    private Action<ArtifactData> _onClick;
    public Action<RectTransform> OnBtnSelected;

    private Color _commonColor = Color.white;
    private Color _unCommonColor = Color.green;
    private Color _rareColor = Color.blue;

    public void OnSelect(BaseEventData eventData)
    {
        OnBtnSelected?.Invoke(_rect);
    }
    public void OnClick() => _onClick?.Invoke(_data);

    public void SetData(ArtifactData data, Action<ArtifactData> onClick)
    {
        _data = data;
        _onClick = onClick;
        
        _icon.sprite = data.icon_sprite;
        _nameTxt.text = data.name_kr;
        _descTxt.text = data.description;
        _rarityTxt.text = data.rarity.ToString();

        _rarityTxt.color = data.rarity switch
        {
            Rarity.Common => _commonColor,
            Rarity.Uncommon => _unCommonColor,
            Rarity.Rare => _rareColor,
            _ => _commonColor // 기본값
        };
    }

}
