using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class InventorySlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler, IDraggableSlot
{
    [SerializeField] protected Image _itemSprite;
    [SerializeField] protected TextMeshProUGUI _countTxt;
    [SerializeField] protected Button _btn;
    [SerializeField] protected RectTransform _rectTransform;
    [SerializeField] protected UI_Inventory _uiInventory;
    [SerializeField] private Image _borderImage; // 테두리용 이미지

    protected GameObject _imageObject;
    protected GameObject _textObject;
    protected InventoryItem _item;
    public InventoryItem Item => _item;

    public int Index { get; set; }
    public bool HasItem => _item != null; //있다면 True

    private Color _normalBorderColor = new Color(1f, 1f, 1f, 0f); // 기본(투명)
    private Color _opaqueColor = new Color(1f, 1f, 1f, 1f); // 기본(투명)
    private Color _highlightBorderColor = Color.yellow; // 강조(노란색)
    private Color _blurColor = new Color(0.7f, 0.7f, 0.7f, 0.5f); // 흐리게
    private Color _selectedBorderColor = Color.blue; // 강조(파란색)
    
    private void ShowIcon() => _imageObject.SetActive(true);
    private void HideIcon() => _imageObject.SetActive(false);
    private void ShowText() => _textObject.SetActive(true);
    private void HideText() => _textObject.SetActive(false);

    public event Action<int> OnSelected;
    public event Action<int> OnDeselected;

    private Dictionary<EInventoryType, Action> _OnClickActions = new(); //Type에 따른 Action 바인딩

    private void Awake()
    {
        _imageObject = _itemSprite.gameObject;
        _textObject = _countTxt.gameObject;

        HideIcon();
        HideText();

        if (_borderImage != null)
            _borderImage.color = _normalBorderColor;
    }

    public void Initialize(UI_Inventory uiInventory)
    {
        _uiInventory = uiInventory;
        // 인벤토리 타입에 따른 Action 매핑
        _OnClickActions[EInventoryType.Inventory] = OnInventoryClick;
        _OnClickActions[EInventoryType.Craft] = OnCraftClick;
        _OnClickActions[EInventoryType.Equipment] = OnEquipmentClick;
    }

    public virtual void OnClickItem() // Call at OnClick Event
    {
        //아이템이 없다면 리턴
        if (!HasItem)
            return;

        _uiInventory.SetSelectIndex(Index);

        //인벤토리타입(키)에 맞는 액션(Value) 찾기
        if (_OnClickActions.TryGetValue(_uiInventory.CurType, out var action))
        {
            //찾았다면 Invoke
            action.Invoke();
        }
        else
        {
            Debug.LogWarning($"{_uiInventory.CurType}에 대한 버튼 액션이 없음.");
        }
    }
    private void OnInventoryClick()
    {
        UIManager.Show<UI_Action>((int)_uiInventory.CurType, _rectTransform, _item, Index);
    }

    private void OnCraftClick()
    {
        Inventory.Instance.Craft(_item);
    }

    private void OnEquipmentClick()
    {
        // 예를 들면 장비 착용창 열기
        UIManager.Show<UI_Action>((int)_uiInventory.CurType, _rectTransform, _item, Index);
    }
    public void SetItem(InventoryItem item) // 슬롯에 아이템 등록
    {
        _item = item;
        if (HasItem)
            _btn.onClick.AddListener(OnClickItem);

        _countTxt.text = _item.Amount.ToString();
        _itemSprite.sprite = _item.GetSprite();
        ShowIcon();
        ShowText();
    }

    public void RemoveItem() // 슬롯에서 아이템제거
    {
        _btn.onClick.RemoveAllListeners();
        _item = null;
        _itemSprite.sprite = null;
        _countTxt.text = string.Empty;
        HideIcon();
        HideText();
    }

    public void ReduceItem(int amount = 1) // 아이템 수량감소
    {
        int i = int.Parse(_countTxt.text);
        i -= amount;
        _countTxt.text = i.ToString();
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        OnSelected?.Invoke(Index);
        _borderImage.color = _selectedBorderColor;
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        OnDeselected?.Invoke(Index);
        SetNormalColor();
    }

    public void SetNormalColor()
    {
        _borderImage.color = _normalBorderColor; //기본색상 적용
    }
    public void SetItemBlurColor()
    {
        _itemSprite.color = _blurColor; 
    }
    public void SetItemNormalColor()
    {
        _itemSprite.color = _normalBorderColor;
    }
    public void SetItemopaque()
    {
        _itemSprite.color = _opaqueColor;
    }

    public void SetHighlight(bool isOn)
    {
        if (_borderImage == null)
        {
            Debug.LogWarning("BorderImage없음.");
            return;
        }

        if (isOn)
        {
            _borderImage.color = _highlightBorderColor; //강조색상 적용
        }
        else
        {
            SetNormalColor();
        }
    }

    public InventoryItem GetItem()
    {
        return _item;
    }
}
