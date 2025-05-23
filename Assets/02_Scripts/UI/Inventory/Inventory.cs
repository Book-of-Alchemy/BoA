using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    //인스펙터창에서 초기화 시키도록 변경예정
    private int _capacity = 28; 

    [SerializeField] private UI_Inventory _uiInventory; // 아래 배열을 UI 상에 보여주는 UI 인벤토리
    [SerializeField] private Alchemy _alchemy; // Inspector로 넣ㅇ는중

    private CraftTool _craftTool;
    public InventoryItem[] items; // 실제 아이템 정보가 있는 배열 해당 배열로 정보이동이 이뤄진다.

    public ItemData[] itemDataArr; // 테스트용 아이템 데이터 배열]

    [SerializeField] private List<InventoryItem> _craftList = new(); // 제작 테이블에 올라간 아이템list
    private List<int> _highlightItemIds = new List<int>(); // 강조되는 아이템 list

    //인벤토리 아이템 유무 bool변수
    public bool HasItem => items != null && items.Any(item => item != null);

    private int _gold = 0;
    public int Gold => _gold;

    public event Action<int> OnGoldChanged;

    public CraftTool GetCraftTool()=> _craftTool;
    protected override void Awake()
    {
        base.Awake();
        // 시작 시 저장된 골드 데이터 로드
        LoadGoldFromDataManager();
    }

    private void Start()
    {
        //초기화 시 불러올 데이터 추가해야함.
        items = new InventoryItem[_capacity]; // 인벤토리 용량만큼 Data상 용량을 맞춰줌.
    }

    public void Initialize(UI_Inventory inventory,CraftTool craftTool) //UI 인벤토리 재생성시 초기화
    {
        _uiInventory = inventory;
        _craftTool = craftTool;

        if (!HasItem) return;
        for (int i = 0; i < items.Length; i++)
        {
            //아이템이 있으면 UI에 갱신
            if (items[i] != null)
            {
                UpdateUISlot(i);
            }
        }
    }

    // DataManager로부터 골드 데이터 로드
    private void LoadGoldFromDataManager()
    {
        if (DataManager.Instance != null)
        {
            // DataManager에서 골드 가져오기
            _gold = DataManager.Instance.GetPlayerData().Gold;
            OnGoldChanged?.Invoke(_gold);
        }
    }

    // DataManager에 골드 데이터 저장
    private void SaveGoldToDataManager()
    {
        if (DataManager.Instance != null)
        {
            // DataManager의 PlayerData에 골드 설정 및 저장
            DataManager.Instance.GetPlayerData().Gold = _gold;
            DataManager.Instance.SaveData();
        }
    }

    public void IncreaseGold(int amount)
    {
        _gold += amount;
        OnGoldChanged?.Invoke(_gold); //골드 변화시 UI에서 갱신
        // 변경된 골드 저장
        SaveGoldToDataManager();
    }

    public void DecreaseGold(int amount)
    {
        _gold = Mathf.Max(0, _gold - amount);
        OnGoldChanged?.Invoke(_gold);
        // 변경된 골드 저장
        SaveGoldToDataManager();
    }

    
    public void SetGold(int amount)
    {
        _gold = Mathf.Max(0, amount);
        OnGoldChanged?.Invoke(_gold);
    }

    // DataManager에서 가져온 골드 관련 메서드
    public void AddGold(int amount)
    {
        _gold += amount;
        OnGoldChanged?.Invoke(_gold);
        
        // 골드 변경 저장
        if (DataManager.Instance != null)
        {
            DataManager.Instance.GetPlayerData().Gold = _gold;
            DataManager.Instance.SaveData();
        }
        
    }

    public bool SpendGold(int amount)
    {
        if (_gold >= amount)
        {
            _gold -= amount;
            OnGoldChanged?.Invoke(_gold);
            
            // 골드 변경 저장
            if (DataManager.Instance != null)
            {
                DataManager.Instance.GetPlayerData().Gold = _gold;
                DataManager.Instance.SaveData();
            }
            
            return true;
        }
        return false;
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
            //가방이 다참.
            if (GameManager.Instance.PlayerTransform != null)
            {
                ItemFactory.Instance.DropItem(itemData.id, GameManager.Instance.PlayerTransform.CurTile, amount);
                UIManager.ShowOnce<UI_Text>("가방이 가득 찼습니다. 아이템을 떨어뜨립니다.");
            }
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

    public int GetItemIndex(int id)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].GetItemId() == id)
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
    public void ClearInventory()//인벤토리 아이템 전부삭제
    {
        if (items == null || items.Length == 0)
            return;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                items[i] = null;
                if(_uiInventory != null)
                    _uiInventory.RemoveItem(i); 
            }
        }
        _highlightItemIds.Clear(); 
    }

    public void DecreaseUseItem(InventoryItem item)// 임시 코드
    {
        int i = FindItemId(item.itemData);
        int index = GetItemIndex(i);
        RemoveItem(index);
    }

    public void FilterAndDisplay(params Item_Type[] types)
    {
        if (!HasItem) return;
        // type 과 enum값이 같은 item을 검색해서 배열로 저장
        InventoryItem[] filtered = Array.FindAll(items, item =>
            item != null && item.itemData != null && Array.Exists(types, type => item.GetItemType() == type));

        // ID 순서로 정렬
        Array.Sort(filtered, (a, b) =>
            string.Compare(a.GetItemId().ToString(), b.GetItemId().ToString()));

        if(filtered.Length != 0)
        {
            _uiInventory.ClearAllSlots();

            for (int i = 0; i < filtered.Length; i++)
                _uiInventory.SetInventorySlot(i, filtered[i]); // UI 슬롯에 재배치
        }
    }
    public List<InventoryItem> GetAllItems()
    {
        //Items 내부 아이템 리턴
        List<InventoryItem> result = new List<InventoryItem>();

        if (items == null)
            return result;

        foreach (var item in items)
        {
            if (item != null && !item.IsEmpty)
                result.Add(item);
        }

        return result;
    }

    public void RestoreBeforeFilter()
    {
        if (!HasItem) return;
        _uiInventory.ClearAllSlots();
        for (int i = 0; i < _uiInventory.SlotCount; i++)
        {
            //_uiInventory.RemoveItem(i); // 슬롯 초기화 및 재배치
            if (items[i] != null)
                _uiInventory.SetInventorySlot(i, items[i]);
        }
    }

    public void TryCraft()
    {
        var validCraftList = _craftList
            .Where(x => x != null && x.itemData != null)
            .ToList();

        if (validCraftList.Count < 2)
        {
            UIManager.ShowOnce<UI_Text>("재료가 부족한 것 같다...");
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
            RemoveCraftList();

        }
        else if(!boolResult)
        {
            UIManager.ShowOnce<UI_Text>("제작에 실패했다.");
        }

    }

    public void RemoveCraftList()
    {
        _craftList.RemoveAll(slot => slot != null);
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
        _craftList.RemoveAll(i => i == null || i.itemData == null || i.GetItemId() == item.GetItemId());
        HighlightCraftableSlots();
        UpdateCraftPreview();
        if (_craftList.Count < 2)
            _craftTool.ClearPreviewSlot();
    }

    public void Use(InventoryItem item, int index)
    {
        if (item.GetItemType() != Item_Type.Consumable)
        {
            UIManager.ShowOnce<UI_Text>("사용가능한 아이템이 아니다.");
            return;
        }
        var controller = GameManager.Instance.PlayerTransform.GetComponent<DungeonBehavior>();
        controller.UseItem(item.itemData);

        RemoveItem(index);
        UIManager.Hide<UI_Action>();
        UIManager.Hide<UI_ItemTooltip>();
        UIManager.Hide<UI_Inventory>();
    }

    public void Drop(InventoryItem item,int index)
    {
        //현재 DropItem은 아이템 EffectType이 DamageType만 가능
        ItemFactory.Instance.DropItem(item.itemData.id, GameManager.Instance.PlayerTransform.CurTile, item.Amount);

        RemoveItem(index, item.Amount);
        UIManager.Hide<UI_Action>();
    }

    public void Craft(InventoryItem item)
    {
        if (_craftList.Count >= 3 || _craftList.Contains(item)) return;

     
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
        UIManager.Hide<UI_Action>();
    }

    public void UnEquip(InventoryItem item)
    {
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
        _highlightItemIds = _alchemy.GetCraftableIds(craftItemIds);

        _uiInventory.CheckHighlight(_highlightItemIds);
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
