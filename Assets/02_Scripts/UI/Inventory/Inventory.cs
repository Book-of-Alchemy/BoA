using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    //인스펙터창에서 초기화 시키도록 변경예정
    private int _capacity = 30; 

    [SerializeField] private UI_Inventory _uiInventory; // 아래 배열을 UI 상에 보여주는 UI 인벤토리
    [SerializeField] private Alchemy _alchemy; // Inspector로 넣ㅇ는중

    private CraftTool _craftTool;
    public InventoryItem[] items; // 실제 아이템 정보가 있는 배열 해당 배열로 정보이동이 이뤄진다.

    public ItemData[] itemDataArr; // 테스트용 아이템 데이터 배열]

    private List<InventoryItem> _craftList = new(); 


    private void Start()
    {
        items = new InventoryItem[_capacity]; // 인벤토리 용량만큼 Data상 용량을 맞춰줌.
    }

    public void Initialize(UI_Inventory inventory,CraftTool craftTool) //UI 인벤토리 재생성시 초기화
    {
        _uiInventory = inventory;
        _craftTool = craftTool;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                Add(items[i].itemData);
        }
    }

    public void OnClickAddItem() //Call at OnClick Event 
    {
        int rand = UnityEngine.Random.Range(0, itemDataArr.Length);
        Add(itemDataArr[rand]);
    }

    public void OnClickRemoveItem() //Call at OnClick Event 
    {
        int index = FindEmptySlotIndex(0, false); // 비어있지 않은 인덱스 검색
        if(index >=0)
            RemoveItem(index);
    }

    public InventoryItem Add(ItemData itemData, int amount = 1, bool isUpdate = true)
    {
        //Add하기전 같은 Id의 아이템이 이미 인벤토리에 있는지 검사하고 있다면 int 리턴
        int index = FindItemId(itemData);
        if(index >= 0)// True라면 찾음
        {
            items[index].AddItem(itemData,amount); //수량증가
            if(isUpdate)
                UpdateUISlot(index);
            return items[index];
        }

        //인벤토리에 해당 Id의 아이템이 없음.
        index = FindEmptySlotIndex();
        if(index >= 0) 
        {
            if (items[index] == null)
                items[index] = new InventoryItem();

            items[index].AddItem(itemData); //빈 공간에 itemData Add
            if(isUpdate)
                UpdateUISlot(index);
            return items[index];
        }
        else
        {
            //빈 슬롯을 못찾았다.
            //가방이 다참.
            return null;
        }
    }

    private void UpdateUISlot(int index) // 해당하는 인덱스 슬롯 상태 및 UI 갱신
    {
        //아이템 배열 Null체크
        if(items[index] != null)
        {
            //index와 일치하는 ui에 item정보 Set
            _uiInventory.SetInventorySlot(index, items[index]);
        }
    }

    private void UpdateUICraft(InventoryItem item) 
    {
        _craftTool.UpdateSlot(item);
    }
    private void RemoveUICraft()
    {
        _craftTool.RemoveAllSlot();
    }


    private int FindEmptySlotIndex(int startIndex = 0 , bool emptyCheck = true) //아이템을 넣을 슬롯을 검색하는 함수
    {
        if (emptyCheck) //비어있다면 해당 인덱스 리턴
            return Array.FindIndex(items, startIndex, item => item == null && emptyCheck);
        else if (!emptyCheck)// 비어있지 않은 index 리턴
            return Array.FindIndex(items, startIndex, item => item != null && !emptyCheck);
        else
            return -1;
    }

    private int FindItemId(ItemData itemData) //ItemData와 ID값이 일치하는 아이템이 있는지 검색
    {
        for (int i = 0; i < _capacity; i++)
        {
            if (items[i] == null) //null이라면 다음 index
                continue;

            if (itemData.id == items[i].GetItemId()) //ItemData가 인벤토리의 i번 id와 일치
                return i;
        }
        return -1;
    }

    private void RemoveItem(int index)
    {
        if(items[index].Amount >= 1)
        {
            if (items[index].GetReuceAmount() == 0)
            {
                items[index] = null;
                _uiInventory.RemoveItem(index);
            }
            else
                _uiInventory.ReduceItem(index);
        }
    }

    public void FilterAndDisplay(params Item_Type[] types)
    {
        // type 과 enum값이 같은 item을 검색해서 배열로 저장
        InventoryItem[] filtered = Array.FindAll(items, item =>
            item != null && Array.Exists(types, type => item.GetItemType() == type));

        // ID 순서로 정렬
        Array.Sort(filtered, (a, b) =>
            string.Compare(a.GetItemId().ToString(), b.GetItemId().ToString()));

        for (int i = 0; i < _uiInventory.SlotCount; i++)
            _uiInventory.RemoveItem(i); // UI 슬롯 초기화

        for (int i = 0; i < filtered.Length/* && i < _uiInventory.SlotCount*/; i++)
            _uiInventory.SetInventorySlot(i, filtered[i]); // UI 슬롯에 재배치
    }

    public void RestoreBeforeFilter()
    {
        for (int i = 0; i < _uiInventory.SlotCount; i++)
        {
            _uiInventory.RemoveItem(i); // 슬롯 초기화 및 재배치
            if (items[i] != null)
                _uiInventory.SetInventorySlot(i, items[i]);
        }
    }
    //public BaseItem Check(int index)
    //{
    //    //var effectType = items[index].itemData.effect_type;
    //    //return factory.UseItem(effectType, this.transform);
    //}

    public void CraftReady()
    {
        var (boolResult, dataResult,amount) = _alchemy.CreateItem(
            _craftList[0].itemData, _craftList[0].Amount, _craftList[1].itemData, _craftList[1].Amount);

        if(boolResult)
        {
            //성공했을 경우 , 기존 Item에서 Amount만큼 감소를 해야하고 Craft창도 비워야함.
            _craftTool.RemoveAllSlot();
            Debug.Log($"{amount}개");
            InventoryItem resultItem = Add(dataResult,amount,false);
            _craftTool.CraftComplete(resultItem);
            _craftList.RemoveAll(slot => slot != null);

        }
        else
        {
            Debug.Log(boolResult);
        }

        Debug.Log(dataResult);


    }

    public void Use(/*int index*/InventoryItem item)
    {
        Debug.Log("UseAction");
        //ItemManager.Instance.CreateProjectileItem(items[index].itemData);

        //RemoveItem(index);
        UIManager.Hide<UI_Action>();
    }

    public void Drop(InventoryItem item)
    {
        Debug.Log("DropAction");
        //RemoveItem(index);
        UIManager.Hide<UI_Action>();
    }

    public void Craft(InventoryItem item)
    {
        //Craft테이블에 item을 같이 넣어야함.
        _craftList.Add(item);
        UpdateUICraft(item);
        UIManager.Hide<UI_Action>();
    }

    public void Cancel(InventoryItem item)
    {
        UIManager.Hide<UI_Action>();
    }

    public void Equip(InventoryItem item)
    {
        Debug.Log("EquipAction");
        UIManager.Hide<UI_Action>();
    }

    public void UnEquip(InventoryItem item)
    {
        Debug.Log("UnEquipAction");
        UIManager.Hide<UI_Action>();
    }
}
