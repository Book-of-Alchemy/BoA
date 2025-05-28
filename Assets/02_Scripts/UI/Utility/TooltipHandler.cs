using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int _itemId;
    public void SetItemId(int id) => _itemId = id;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var data = SODataManager.Instance.itemDataBase.GetItemDataById(_itemId);
        if (data != null)
            UIManager.Show<UI_ItemTooltip>(data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Hide<UI_ItemTooltip>();
    }
}
