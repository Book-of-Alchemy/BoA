using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactSlotUI : SlotUIBase<ArtifactData>, ISelectHandler
{
    [SerializeField] private RectTransform _rect;
    public Button _btn;

    public Action<RectTransform,ArtifactData> OnBtnSelected;

    public void OnSelect(BaseEventData eventData)
    {
        OnBtnSelected?.Invoke(_rect,Data);
    }
    protected override void ShowTooltip(ArtifactData data)
    {

    }
    public override void OnClick()
    {
        
    }

    protected override void UpdateUI(ArtifactData data)
    {
        _icon.sprite = data?.icon_sprite;
        _icon.enabled = data != null;
    }

    protected override void ClearUI()
    {
        _icon.sprite = null;
        _icon.enabled = false;
    }
}
