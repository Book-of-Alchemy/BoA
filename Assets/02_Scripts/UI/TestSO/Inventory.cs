using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int _capacity = 30;

    [SerializeField] private UI_Inventory _uiInventory; // 아래 배열을 UI 상에 보여주는 UI 인벤토리
    [SerializeField] private InventoryItem[] _items; // 실제 아이템 정보가 있는 배열 해당 배열로 정보이동이 이뤄진다.

    public InventoryItemData[] ItemDataArr; // 테스트용 아이템 데이터 배열

    private void Start()
    {
        _items = new InventoryItem[_capacity]; // 인벤토리 용량만큼 Data상 용량을 맞춰줌.
    }

    public void OnClickAddItem() //Call at OnClick Event 
    {
        int rand = Random.Range(0, ItemDataArr.Length);
        Add(ItemDataArr[rand]);
    }

    public void Add(InventoryItemData itemData, int amount = 1)
    {
        int index = FindEmptySlotIndex();
        if(index >= 0) // 0보다 크거나 같다면 EmptySlot을 찾음.
        {
            _items[index] = new InventoryItem();
            _items[index].AddItem(itemData); //빈 공간에 itemData Add
            _items[index].AddAmount(amount);
            UpdateUISlot(index);
        }
        else
        {
            //빈 슬롯을 못찾았다.
        }
    }

    private void UpdateUISlot(int index) // 해당하는 인덱스 슬롯 상태 및 UI 갱신
    {
        //아이템이 있다면
        if(_items[index] != null)
        {
            //아이콘 Set
            _uiInventory.SetSlotItem(_items[index].InventorItemData.Icon,index);
            //Icon구분없이 Update될수있게
        }
    }

    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < _capacity; i++)
            if (_items[i] == null)
                return i; //[i]가 비어있다면 해당 인덱스 리턴
        return -1; //startIndex부터 전체를 순회했지만 빈 자리가 없다면 -1리턴
    }
}
