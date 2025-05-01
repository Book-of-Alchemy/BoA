using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    [SerializeField] private List<QuickSlotUI> _slots;

    private void Awake()
    {
        for (int i = 0; i < _slots.Count; i++) 
        {
            _slots[i].Initialize(i+1);// slot에 인덱스 번호 +1
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _slots[0].UseItem();
        }
    }

}
