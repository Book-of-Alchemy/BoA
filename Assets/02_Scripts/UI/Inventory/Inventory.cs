using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    //인스펙터창에서 초기화 시키도록 변경예정
    private int _capacity = 30; 

    [SerializeField] private UI_Inventory _uiInventory; // 아래 배열을 UI 상에 보여주는 UI 인벤토리
    public InventoryItem[] items; // 실제 아이템 정보가 있는 배열 해당 배열로 정보이동이 이뤄진다.

    public ItemData[] itemDataArr; // 테스트용 아이템 데이터 배열]

    private void Start()
    {
        items = new InventoryItem[_capacity]; // 인벤토리 용량만큼 Data상 용량을 맞춰줌.
    }

    public void Initialize(UI_Inventory inventory) //UI 인벤토리 재생성시 초기화
    {
        _uiInventory = inventory;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                Add(items[i].BaseItem.data);
        }
    }

    public void OnClickAddItem() //Call at OnClick Event 
    {
        int rand = Random.Range(0, itemDataArr.Length);
        Add(itemDataArr[0]);
    }

    public void OnClickRemoveItem() //Call at OnClick Event 
    {
        RemoveItem();
    }

    public void Add(ItemData itemData, int amount = 1)
    {
        //Add하기전 같은 Id의 아이템이 이미 인벤토리에 있는지 검사하고 있다면 int 리턴
        int index = FindItemId(itemData);
        if(index >= 0)// 0보다 크거나 같다면 찾음.
        {
            //items[index].AddItem(itemData,amount); //겹치기때문에 수량증가
            items[index].AddBaseItem(itemData,amount);
            UpdateUISlot(index, items[index]);
            return;
        }

        //인벤토리에 해당 Id의 아이템이 없음.
        index = FindEmptySlotIndex();
        if(index >= 0) 
        {
            if (items[index] == null)
                items[index] = new InventoryItem();

            items[index].AddBaseItem(itemData); //빈 공간에 itemData Add
            UpdateUISlot(index, items[index]);
        }
        else
        {
            //빈 슬롯을 못찾았다.
        }
    }

    private void UpdateUISlot(int index,InventoryItem item) // 해당하는 인덱스 슬롯 상태 및 UI 갱신
    {
        //아이템 배열 Null체크
        if(items[index] != null)
        {
            //index와 일치하는 ui에 item정보 Set
            _uiInventory.SetSlotItem(index, item);
            
        }
    }

    private int FindEmptySlotIndex(int startIndex = 0) //아이템을 넣을 슬롯을 검색하는 함수
    {
        for (int i = startIndex; i < _capacity; i++)
            if (items[i] == null)
                return i; //[i]가 비어있다면 해당 인덱스 리턴
        return -1; //빈 자리가 없다면 -1리턴
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

    private void RemoveItem()
    {

    }

    public void Use(int index)
    {
        Debug.Log("UseAction");
        UIManager.Hide<UI_Action>();
    }

    public void Drop(int index)
    {
        Debug.Log("DropAction");
        UIManager.Hide<UI_Action>();
    }
    public void Craft(int index)
    {
        Debug.Log("CraftAction");
        UIManager.Hide<UI_Action>();
    }

    public void Cancel(int index)
    {
        Debug.Log("CancelAction");
        UIManager.Hide<UI_Action>();
    }

    public void Equip(int index)
    {
        Debug.Log("EquipAction");
        UIManager.Hide<UI_Action>();
    }

    public void UnEquip(int index)
    {
        Debug.Log("UnEquipAction");
        UIManager.Hide<UI_Action>();
    }
}
