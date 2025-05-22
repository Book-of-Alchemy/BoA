using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    [SerializeField] private List<QuickSlotUI> _slots;
    public List<QuickSlotUI> Slots => _slots;

    private void Awake()
    {
        for (int i = 0; i < _slots.Count; i++) 
        {
            _slots[i].Initialize(i+1);// slot에 인덱스 번호 +1
        }
    }

    private void OnEnable()
    {
        InputManager.Instance.OnQuickSlotUse += HandleQuickSlotUse;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnQuickSlotUse -= HandleQuickSlotUse;
    }
    private void HandleQuickSlotUse(int index)
    {
        // 1-based index로 받았기 때문에 -1 처리
        int slotIndex = index - 1;
        if (slotIndex >= 0 && slotIndex < _slots.Count)
        {
            _slots[slotIndex].UseItem();
        }
    }
}
