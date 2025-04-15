using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotSelect : MonoBehaviour , ISelectHandler, IDeselectHandler
{
    public Slots slot;

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        if(slot._item.TestItemData != null)
            UI_Inventory.Instance.OnSlotSelected(slot);
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        UI_Inventory.Instance.OnSlotDeselected();
    }
}
