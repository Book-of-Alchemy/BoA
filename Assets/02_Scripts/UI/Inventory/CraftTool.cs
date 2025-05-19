using System.Collections.Generic;
using UnityEngine;

public class CraftTool : MonoBehaviour
{
    [Header("CraftSlot")]
    [SerializeField] private List<CraftSlotUI> _slotCraft;
    [SerializeField] private InventorySlotUI _slotResult;

    public void UpdateSlot(InventoryItem item)
    {
        if (_slotCraft[2].HasData)//가득찼다면 더 이상 업데이트 X
        {
            return;
        }

        int index = FindSlot();
        _slotCraft[index].Index = index;
        _slotCraft[index].SetData(item);
    }

    public void RemoveCraftSlot() // Craft 슬롯 비우기
    {
        foreach (var slot in _slotCraft)
        {
            slot.RemoveData();
        }

    }
    public void RemoveAllSlot()
    {
        foreach (var slot in _slotCraft)
        {
            slot.RemoveData();
        }

        _slotResult.RemoveData();
    }

    public void SetPreviewSlot(InventoryItem item, int amount)
    {
        _slotResult.SetData(item);
        _slotResult.SetItemBlurColor();
    }

    public void ClearPreviewSlot()
    {
        _slotResult.RemoveData();
    }
    private int FindSlot()
    {
        return _slotCraft.FindIndex(0,slot => !slot.HasData);
    }

    public void CraftItem() // 인스펙터에서 등록됨.
    {
        Inventory.Instance.TryCraft();
    }

    public void CraftComplete(InventoryItem item)
    {
        ClearPreviewSlot();
        _slotResult.SetData(item);
        _slotResult.SetItemopaque();
    }
}
