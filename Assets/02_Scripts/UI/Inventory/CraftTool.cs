using System.Collections.Generic;
using UnityEngine;

public class CraftTool : MonoBehaviour
{
    [Header("CraftSlot")]
    [SerializeField] private List<CraftSlotUI> _slotCraft;
    [SerializeField] private InventorySlotUI _slotResult;

    public void UpdateSlot(InventoryItem item)
    {
        if (_slotCraft[2].HasItem)//가득찼다면 더 이상 업데이트 X
        {
            return;
        }

        int index = FindSlot();
        _slotCraft[index].Index = index;
        _slotCraft[index].SetItem(item);
    }

    public void RemoveAllSlot() // Craft 슬롯 비우기
    {
        foreach (var slot in _slotCraft)
        {
            slot.RemoveItem();
        }

    }

    private int FindSlot()
    {
        return _slotCraft.FindIndex(0,slot => !slot.HasItem);
    }

    public void CraftItem() // 인스펙터에서 등록됨.
    {
        Inventory.Instance.CraftReady();
    }

    public void CraftComplete(InventoryItem item)
    {
        _slotResult.SetItem(item);
    }
}
