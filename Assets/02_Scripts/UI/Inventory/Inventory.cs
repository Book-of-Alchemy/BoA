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

    private List<InventoryItem> _craftList = new(); // 제작 테이블에 올라간 아이템list
    private List<int> _highlightItemIds = new List<int>(); // 강조되는 아이템 list

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
                UpdateUISlot(i);
        }
    }

    public void OnClickAddItem() //Call at OnClick Event 
    {
        int rand = UnityEngine.Random.Range(0, itemDataArr.Length);
        //Add(itemDataArr[rand],10);
        foreach (var item in itemDataArr)
        {
            Add(item, 10);
        }

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
            if(isUpdate && _uiInventory != null)
                UpdateUISlot(index);
            return items[index];
        }

        //인벤토리에 해당 Id의 아이템이 없음.
        index = FindEmptySlotIndex();
        if(index >= 0) 
        {
            if (items[index] == null)
                items[index] = new InventoryItem();

            items[index].AddItem(itemData,amount); //빈 공간에 itemData Add

            if(isUpdate && _uiInventory != null)
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

    private int GetItemIndex(int id)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItemId() == id)
                return i;
        }

        return -1;
    }

    private void RemoveItem(int index, int amount = 1)
    {
        if(items[index].Amount >= 1)
        {
            int value = items[index].DecreaseAmount(amount);
            if (value == 0)
            {
                items[index] = null;
                _uiInventory.RemoveItem(index);
            }
            else
                _uiInventory.ReduceItem(index,amount);

        }
    }

    public void DecreaseUseItem(InventoryItem item)// 임시 코드
    {
        int i = FindItemId(item.itemData);
        int index = GetItemIndex(i);
        RemoveItem(index);
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

    public void TryCraft()
    {
        if (_craftList.Count < 1)
        {
            return;
        }
        //제작 결과 담을 변수
        (bool boolResult, RecipeData dataResult, int amount) result;

        if (_craftList.Count >= 3)
        {
            result = _alchemy.CreateItem(
            _craftList[0].itemData, _craftList[0].Amount,
            _craftList[1].itemData, _craftList[1].Amount,
            _craftList[2].itemData, _craftList[2].Amount);
        }
        else // 1개를 넣고 제작을 시도했을때 처리도 추가해야함.
        {
            result = _alchemy.CreateItem(
            _craftList[0].itemData, _craftList[0].Amount,
            _craftList[1].itemData, _craftList[1].Amount);
        }
        var (boolResult, dataResult, amount) = result;

        if (boolResult) // 제작에 성공했을 경우
        {
            //성공했을 경우,기존 Item에서 Amount만큼 감소를 해야하고 Craft창도 비워야함.
            _craftTool.RemoveCraftSlot(); // UI제작테이블 비우기
            ItemData item = ResourceManager.Instance.dicItemData[dataResult.output_item_id];

            InventoryItem resultItem = Add(item, amount, false);
            _craftTool.CraftComplete(resultItem); //결과 슬롯에 제작 성공아이템 전달

            var materials = new List<(int id, int amo)>
            {
                (dataResult.material_1_item_id, dataResult.material_1_amount),
                (dataResult.material_2_item_id, dataResult.material_2_amount),
                (dataResult.material_3_item_id, dataResult.material_3_amount),
            };

            foreach (var (id, amo) in materials)
            {
                if(id >= 0) // 3번째 비어있을시
                {
                    int index = GetItemIndex(id);
                    RemoveItem(index, amo);
                }
            }
            _craftList.RemoveAll(slot => slot != null);

        }
        else
        {
            Debug.Log(boolResult);
        }

        //Debug.Log(dataResult);
    }
    private void UpdateCraftPreview()
    {
        //2개 이상부터 검사
        if (_craftList.Count < 1 ) return;

        List<int> craftItemIds = new List<int>();
        foreach (var item in _craftList)
        {
            if (item != null)
                craftItemIds.Add(item.GetItemId());
        }

        InventoryItem resultItem = _alchemy.GetCraftResultPreview(craftItemIds);

        if(resultItem != null)
        {
            _craftTool.SetPreviewSlot(resultItem, resultItem.Amount);
        }
        else
        {
            _craftTool.ClearPreviewSlot();
        }
    }
    public void RemoveCraftTable(InventoryItem item)
    {
        _craftList.Remove(item);
        HighlightCraftableSlots();
        UpdateCraftPreview();
    }

    public void Use(InventoryItem item, int index)
    {
        Debug.Log("UseAction");
        var controller = GameManager.Instance.PlayerTransform.GetComponent<DungeonBehavior>();
        controller.UseItem(item.itemData);

        RemoveItem(index);
        UIManager.Hide<UI_Action>();
        UIManager.Hide<UI_Inventory>();
    }

    public void Drop(InventoryItem item,int index)
    {
        Debug.Log("DropAction");
        //현재 DropItem은 아이템 EffectType이 DamageType만 가능
        BaseItem baseItem = ItemManager.Instance.CreateItem(item.itemData);
        baseItem.DropItem(item.itemData,item.Amount);

        RemoveItem(index, item.Amount);
        UIManager.Hide<UI_Action>();
    }

    public void Craft(InventoryItem item)
    {
        //Craft테이블에 item을 같이 넣어야함.
        _craftList.Add(item);

        HighlightCraftableSlots();
        UpdateUICraft(item);
        UpdateCraftPreview();
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

    public List<int> GetHighlightItemIds()
    {
        return _highlightItemIds;
    }

    //제작테이블에 올라간 아이템과 제작이 가능한 아이템 강조
    //craftList에 먼저 아이템이 올라간 후에 동작되어야함.
    public void HighlightCraftableSlots()
    {
        if (_craftList.Count == 0)
        {
            ClearAllHighlights(); 
            return;
        }
        ClearAllHighlights(); // 모든 강조 초기화

        //현재 _craftList에 올라간 모든 아이템 ID를 저장
        HashSet<int> craftItemIds = new HashSet<int>();
        foreach (var item in _craftList)
        {
            if (item != null)
                craftItemIds.Add(item.GetItemId());
        }

        //현재 _craftList에 올라간 craftItemIds 기준으로 제작 가능한 ID 리턴
        List<int> requiredItemIds = _alchemy.GetCraftableIds(craftItemIds);

        _highlightItemIds = requiredItemIds;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                continue;

            //인벤토리에서 강조할 아이템 대조 검사
            if (requiredItemIds.Contains(items[i].GetItemId()))
            {
                _uiInventory.HighlightSlot(i);
            }
        }
    }

    public void ClearAllHighlights()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                continue;

            _uiInventory.UnhighlightSlot(i);
        }
        _highlightItemIds.Clear();
    }

    public void SetItemAtoB(int indexA,int indexB )
    {
        //B인덱스가 비어있다면 A를 B로 옮기기
        if (items[indexB] == null)
        {
            InventoryItem temp = items[indexA];
            items[indexB] = temp;
            items[indexA] = null;
            _uiInventory.SetInventorySlot(indexB, items[indexB]);
            _uiInventory.RemoveItem(indexA);
        }
        else
        {
            _uiInventory.SetInventorySlot(indexA, items[indexB]);
            _uiInventory.SetInventorySlot(indexB, items[indexA]);
            InventoryItem temp = items[indexA];
            items[indexA] = items[indexB];
            items[indexB] = temp;

        }

    }
}
