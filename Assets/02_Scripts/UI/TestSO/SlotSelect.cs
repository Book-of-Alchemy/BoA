using UnityEngine;
using UnityEngine.EventSystems;

public class SlotSelect : MonoBehaviour , ISelectHandler, IDeselectHandler
{
    public InventorySlotUI slot;

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
    }
}
