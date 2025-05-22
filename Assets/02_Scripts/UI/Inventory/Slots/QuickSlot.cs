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

    private void Update()
    {
        //테스트용 코드 PlayerInput과 연결예정
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _slots[0].UseItem();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _slots[1].UseItem();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _slots[2].UseItem();
        }
    }

}
