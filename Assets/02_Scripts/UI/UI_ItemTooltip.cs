using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_ItemTooltip : UIBase
{
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _description;

    [SerializeField] private RectTransform _tooltipRect;
    [SerializeField] private CanvasGroup _canvasGroup;

    private Canvas _canvas;
    public override void Opened(params object[] param)
    {
        if (param == null || param.Length == 0 || !(param[0] is ItemData data))
        {
            Debug.LogError("Invalid param for tooltip");
            return;
        }
        _canvasGroup.blocksRaycasts = false;
        _canvas = UIManager.Instance.GetCanvas();
        Debug.Log(_canvas);

        _itemName.text = data.name_kr;
        _description.text = data.iteminfo_kr;

        SetPositionToMouse();
    }

    private void SetPositionToMouse()
    {
        //if(_canvas == null)
        //    Canvas canvas = GetComponentInParent<Canvas>();
        //else
        var canvas = _canvas;


        Vector2 tooltipSize = _tooltipRect.sizeDelta;
        Vector2 pivot = _tooltipRect.pivot;

        Vector2 anchoredPos;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            anchoredPos = Input.mousePosition;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out var localPoint);

            anchoredPos = ((RectTransform)canvas.transform).TransformPoint(localPoint);
        }
        else return;

        // 오프셋 추가
        anchoredPos += new Vector2(20f, -20f);

        // 툴팁이 스크린 바깥으로 나가지 않도록 Clamp
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float leftBound = 0 + tooltipSize.x * pivot.x;
        float rightBound = screenWidth - tooltipSize.x * (1f - pivot.x);
        float topBound = screenHeight - tooltipSize.y * (1f - pivot.y);
        float bottomBound = 0 + tooltipSize.y * pivot.y;

        anchoredPos.x = Mathf.Clamp(anchoredPos.x, leftBound, rightBound);
        anchoredPos.y = Mathf.Clamp(anchoredPos.y, bottomBound, topBound);

        _tooltipRect.position = anchoredPos;
    }

    public override void HideDirect()
    {
        
    }
}
