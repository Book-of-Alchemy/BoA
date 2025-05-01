using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EInventoryType
{
    Inventory,
    Craft,
    Equipment
}

public class UI_Inventory : UIBase
{
    [Header("Inventory")]
    [SerializeField] private ItemInfo _itemInfo;
    [SerializeField] private Inventory _inventory; //데이터를 가지고 있는 인벤토리
    [SerializeField] private List<InventorySlotUI> _slotUIList;  //인벤토리 UI가 가지고있는 SlotUI를 리스트로 가지고 있음.

    [Header("Tools")]
    [Tooltip("Connect with Enum")]
    [SerializeField] private List<GameObject> _toolList; //UI_Inventory에 오른쪽에 바뀔 화면
    [SerializeField] private List<Image> _imageList; //화면전환을 위한 탭버튼의 이미지 

    [Header("Test Comp")]
    [SerializeField] private Button _addBtn;
    [SerializeField] private Button _removeBtn;
    [SerializeField] private Button _sortBtn;
    [SerializeField] private Button _sortResetBtn;
    public int SlotCount => _slotUIList.Count;

    public event Action<EInventoryType> OnInventoryChanged;
    private EInventoryType _curType; // 현재 띄워진 인벤토리 타입
    public EInventoryType CurType
    {
        get => _curType;
        set
        {
            if (_curType == value) return;

            _curType = value; 
            OnInventoryChanged?.Invoke(_curType);
        }
    }

    private Dictionary<EInventoryType, Item_Type[]> _typeFilter= new() //인벤토리 타입에 따른 Item필터타입
    {
        { EInventoryType.Craft, new[] { Item_Type.Material, /*Item_Type.Consumable*/} },
    };

    public int SelectIndex { get; private set; }
    public bool IsOpened { get; private set; }

    private Color _unActiveColor = Color.gray;
    private Color _activeColor; 

    private CraftTool _craftTool;

    private void Start()
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            _slotUIList[i].Initialize(this);
            var slot = _slotUIList[i];
            slot.Index = i;

            slot.OnSelected += OnSlotSelected;
            slot.OnDeselected += OnSlotDeselected;
        }
        
    }

    private void InitInventoryType() //시작시 버튼 컬러 초기화
    {
        _activeColor = _imageList[0].color; //기존 컬러 저장

        for (int i = 0; i < _imageList.Count; i++)
        {
            _imageList[i].color = _unActiveColor; 
        }
    }

    private void InitInventory()
    {
        if (Inventory.Instance != null)
        {
            _inventory = Inventory.Instance;
            _craftTool = _toolList[1].GetComponent<CraftTool>();
            _inventory.Initialize(this,_craftTool);
            _addBtn.onClick.AddListener(_inventory.OnClickAddItem);
            _removeBtn.onClick.AddListener(_inventory.OnClickRemoveItem);
            _sortBtn.onClick.AddListener(() => _inventory.FilterAndDisplay(Item_Type.Material));
            _sortResetBtn.onClick.AddListener(() => _inventory.RestoreBeforeFilter());
        }
    }

    public override void HideDirect() //Call at OnClick Event 
    {
        _addBtn.onClick.RemoveAllListeners();
        _removeBtn.onClick.RemoveAllListeners();
        _sortBtn.onClick.RemoveAllListeners();
        _sortResetBtn.onClick.RemoveAllListeners();
        UIManager.Hide<UI_Inventory>();
        UIManager.Hide<UI_Action>();
    }

    public override void Opened(params object[] param)
    {
        IsOpened = true;
        InitInventoryType();
        InitInventory();

        if (param.Length > 0 && param[0] is EInventoryType)
        {
            SetCurType((EInventoryType)param[0]); 
            ShowRightTool(CurType);
        }
        else
        {
            Debug.Log("Add Param EInventoryType");
        }
    }

    public void SetInventorySlot(int index, InventoryItem item) //슬롯에 아이템 UI 갱신
    {
        _slotUIList[index].SetItem(item);
    }

    public void RemoveItem(int index) // 슬롯에 아이템 아이콘, 갯수 제거
    {
        _slotUIList[index].RemoveItem();
    }
    public void ReduceItem(int index,int amount) // 슬롯에 아이템 아이콘, 갯수 제거
    {
        _slotUIList[index].ReduceItem(amount);
    }

    public void OnSlotSelected(int index)
    {
        if (_inventory.items[index] != null)
            _itemInfo.ShowInfo(_inventory.items[index]);
    }

    public void OnSlotDeselected(int index)
    {
        _itemInfo.ClearInfo();
    }

    public void ShowRightTool(EInventoryType type) //어떤 InventoryTool을 사용할지 입력을 받아서 보여주는 역할
    {
        for (int i = 0; i < _toolList.Count; i++)
        {
            if(i == (int)type)
            {
                SetCurType((EInventoryType)i);
                _toolList[i].gameObject.SetActive(true);
                _imageList[i].color = _activeColor;
                UIManager.Hide<UI_Action>();
            }
            else
            {
                _toolList[i].gameObject.SetActive(false);
                _imageList[i].color = _unActiveColor;
            }
        }
    }

    private void SetCurType(EInventoryType type) // 현재 타입 설정 및 인벤토리 필터링
    {
        if (_curType == type) return;

        _curType = type;
        Inventory.Instance.ClearAllHighlights();

        //키에 맞는 Type찾아서 인벤토리 필터링
        if (_typeFilter.TryGetValue(type, out Item_Type[] types))
            _inventory.FilterAndDisplay(types);
        else //기본 인벤토리는 필터링 없기때문에 
            _inventory.RestoreBeforeFilter();
    }

    public void HighlightSlot(int index)
    {
        //index범위체크
        if (index < 0 || index >= SlotCount)
            return;

        //bool 변수를 이용해 강조 끄고 키기
        _slotUIList[index].SetHighlight(true);
    }
    public void UnhighlightSlot(int index)
    {
        if (index < 0 || index >= SlotCount)
            return;

        _slotUIList[index].SetHighlight(false);
    }

    public void OnClickInventoryBtn()
    {
        ShowRightTool(EInventoryType.Inventory);
    }
    public void OnClickCraftBtn()
    {
        ShowRightTool(EInventoryType.Craft);
    }
    public void OnClickEquipmentBtn()
    {
        ShowRightTool(EInventoryType.Equipment);
    }

    public void SetSelectIndex(int index)
    {
        SelectIndex = index;
    }
}
