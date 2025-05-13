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
    [SerializeField] private RectTransform _rect;
    public Button _btn;

    private ArtifactData _data;
    private Action<ArtifactData> _onClick;
    public Action<RectTransform> OnBtnSelected;

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
    }

}
