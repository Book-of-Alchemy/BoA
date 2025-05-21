using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EInventoryType //탭에 따라 보여질 Window 타입
{
    Status,
    Equipment,
    Craft,
    Inventory,
    Quest,
    Map,
    Artifact,
    None,
}

public class UI_Inventory : UIBase
{
    [Header("Inventory")]
    [SerializeField] private ItemInfo _itemInfo;
    [SerializeField] private Inventory _inventory; //데이터를 가지고 있는 인벤토리
    [SerializeField] private List<InventorySlotUI> _slotUIList;  //인벤토리 UI가 가지고있는 SlotUI를 리스트로 가지고 있음.

    [Header("Windows")]
    [Tooltip("Connect with Enum")]
    [SerializeField] private List<GameObject> _windowList; //UI_Inventory에 오른쪽에 바뀔 화면
    [SerializeField] private List<Image> _tabList; //화면전환을 위한 탭버튼의 이미지 
    [SerializeField] private GameObject _commonWindow; 

    [Header("Test Comp")]
    [SerializeField] private Button _addBtn;
    [SerializeField] private Button _removeBtn;
    [SerializeField] private Button _sortBtn;
    [SerializeField] private Button _sortResetBtn;

    [SerializeField] private UIAnimator _uiAnimator;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _canvasGroup;
    public int SlotCount => _slotUIList.Count;

    public event Action<EInventoryType> OnInventoryChanged;
    private EInventoryType _curType = EInventoryType.None; // 현재 띄워진 인벤토리 타입
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
        { EInventoryType.Craft, new[] { Item_Type.Material, } },
        { EInventoryType.Equipment, new[] { Item_Type.Special, } },
    };
    public bool IsOpened { get; private set; }

    private Color _unActiveColor = Color.gray;
    private Color _activeColor;
    private bool _isFirstTypeSet = true;

    private CraftTool _craftTool;
    private EInventoryType _showType;

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
        _activeColor = _tabList[0].color; //기존 컬러 저장

        for (int i = 0; i < _tabList.Count; i++)
        {
            _tabList[i].color = _unActiveColor; 
        }
    }

    private void InitInventory()
    {
        if (Inventory.Instance != null)
        {
            _inventory = Inventory.Instance;
            _craftTool = _windowList[2].GetComponent<CraftTool>();
            _inventory.Initialize(this,_craftTool);
            //버튼 등록
            _addBtn.onClick.AddListener(_inventory.OnClickAddItem);
            _removeBtn.onClick.AddListener(_inventory.OnClickRemoveItem);
            _sortBtn.onClick.AddListener(() => _inventory.FilterAndDisplay(Item_Type.Material));
            _sortResetBtn.onClick.AddListener(() => _inventory.RestoreBeforeFilter());
        }
    }

    public override void HideDirect() //Call at OnClick Event 
    {
        _uiAnimator.FadeOut(OnHide);//애니메이션 전에 인벤토리 하위 숨기기
        _addBtn.onClick.RemoveAllListeners();
        _removeBtn.onClick.RemoveAllListeners();
        _sortBtn.onClick.RemoveAllListeners();
        _sortResetBtn.onClick.RemoveAllListeners();
    }

    public override void Opened(params object[] param)
    {
        _canvasGroup.SetActive(false);

        IsOpened = true;
        _animator.SetTrigger("Open"); //인벤토리 여는 애니메이션

        //인벤토리 초기화
        InitInventoryType();
        InitInventory(); //인벤토리 UI 초기화
   
        if (param.Length > 0 && param[0] is EInventoryType)
        {
            _showType = (EInventoryType)param[0];
        }
        else
        {
            Debug.Log("Add Param EInventoryType");
        }
    }

    public void CheckHighlight(List<int> ids)
    {

        for (int i = 0; i < _slotUIList.Count; i++)
        {
            if (!_slotUIList[i].HasData) continue;

            if (ids.Contains(_slotUIList[i].Data.GetItemId()))
            {
                _slotUIList[i].SetHighlight(true);
            }
        }
    }

    private void OnHide() 
    {
        //모든 하위요소를 숨겼다면 Close
        UIManager.Hide<UI_Action>();
        _animator.SetTrigger("Close");
    }

    public void BookOpened() //Open Animation 이벤트에서 호출
    {
        //열렸다면 모든 하위요소 FadeIn
        _uiAnimator.FadeIn(()=> ShowRightTool(_showType));
    }

    public void BookClosed()
    {
        UIManager.Hide<UI_Inventory>();
    }

    public void SetInventorySlot(int index, InventoryItem item) //슬롯에 아이템 UI 갱신
    {
        _slotUIList[index].SetData(item);
    }

    public void RemoveItem(int index) // 슬롯에 아이템 아이콘, 갯수 제거
    {
        var slot = _slotUIList[index];
        //빈슬롯이면 return
        if (!slot.HasData) return;
        slot.RemoveData();
    }

    public void ReduceItem(int index,int amount) // 슬롯에 아이템 아이콘, 갯수 제거
    {
        _slotUIList[index].ReduceItem(amount);
    }
    public void ClearAllSlots()
    {
        foreach (var slot in _slotUIList)
            slot.RemoveData();
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
        for (int i = 0; i < _windowList.Count; i++)
        {
            if(i == (int)type) 
            {
                //type에 맞는 InventoryTool 노출
                SetCurType(type);
                //_toolList[i].gameObject.SetActive(true);
                _tabList[i].color = _activeColor;
                UIManager.Hide<UI_Action>();
            }
            else
            {
                _windowList[i].gameObject.SetActive(false);
                _tabList[i].color = _unActiveColor;
            }
        }
    }

    private void SetCurType(EInventoryType type) // 현재 타입 설정 및 인벤토리 필터링
    {
        if (_curType == type) return;

        _curType = type;

        if (!_isFirstTypeSet) //초기에는 Flip 예외
            _uiAnimator.FadeOut(OnPageFlip);
        else
            ActiveWindow();
        
        Inventory.Instance.ClearAllHighlights();

        //현재 Type의 값(필터링할 아이템 타입들) 찾아서 인벤토리 필터링
        if (_typeFilter.TryGetValue(type, out Item_Type[] types))
            _inventory.FilterAndDisplay(types);
        else //기본 인벤토리는 필터링 없기때문에 
            _inventory.RestoreBeforeFilter();

        foreach (var item in types)
        {
            Debug.Log(item);
        }
        _isFirstTypeSet = false;
    }

    private void OnPageFlip()
    {
        ActiveWindow();

        // 확률적으로 책넘기는 Animation 실행
        int rand = UnityEngine.Random.value < 0.5f ? 0 : 1;
        if(rand == 0)
        {
            _animator.SetTrigger("FlipLeft"); //OnFliped호출
        }
        else
        {
            _animator.SetTrigger("FlipRight");//OnFliped호출
        }
    }

    private void ActiveWindow()
    {
        _windowList[(int)_curType].gameObject.SetActive(true);
        if (_curType switch // type이 _commonWindow가 필요한 인벤토리 타입인지 검사
        {
            EInventoryType.Craft or EInventoryType.Inventory or EInventoryType.Equipment => true,
            _ => false
        }) //조건문 끝
        {
            _commonWindow.SetActive(true); //필요한 타입이라면 공용Window 활성화
        }
        else
        { 
            _commonWindow.SetActive(false); // 아니라면 비활성화
        }
    }
    
    private void OnFliped()// Call At FlipLeft,Right Animation Event
    {
        _uiAnimator.FadeIn();
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

    //Inspector 연결된 버튼이벤트
    public void OnClickStatusBtn() => ShowRightTool(EInventoryType.Status);
    public void OnClickEquipmentBtn() => ShowRightTool(EInventoryType.Equipment);
    public void OnClickCraftBtn() => ShowRightTool(EInventoryType.Craft);
    public void OnClickInventoryBtn() => ShowRightTool(EInventoryType.Inventory);
    public void OnClickQuestBtn() => ShowRightTool(EInventoryType.Quest);
    public void OnClickMapBtn() => ShowRightTool(EInventoryType.Map);
    public void OnClickArtifactBtn() => ShowRightTool(EInventoryType.Artifact);
}
