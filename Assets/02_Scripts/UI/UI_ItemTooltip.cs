using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_ItemTooltip : UIBase
{
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _description;

    public override void Opened(params object[] param)
    {
        if (param == null || param.Length == 0 || !(param[0] is ItemData data))
        {
            Debug.LogError("Invalid param for tooltip");
            return;
        }

        _itemName.text = data.name_kr;
        _description.text = data.iteminfo_kr;

        SetPositionToMouse();
    }

    private void SetPositionToMouse()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform rect = GetComponent<RectTransform>();

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rect.position = Input.mousePosition;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out var localPoint);
            rect.localPosition = localPoint;
        }
    }
    public override void HideDirect()
    {
        
    }
}
