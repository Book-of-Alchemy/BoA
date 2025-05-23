using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class InventorySlotUI : SlotUIBase<InventoryItem>, ISelectHandler, IDeselectHandler, IDraggableSlot
{
    [SerializeField] protected Button _btn;
    [SerializeField] protected RectTransform _rectTransform;
    [SerializeField] protected UI_Inventory _uiInventory;
    [SerializeField] private Image _borderImage; // 테두리용 이미지

    protected GameObject _imageObject;
    protected GameObject _textObject;

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
        _imageObject = _icon.gameObject;
        _textObject = _countTxt.gameObject;

        HideIcon();
        HideText();

        if (_borderImage != null) _borderImage.color = _normalBorderColor;
    }

    public void Initialize(UI_Inventory uiInventory)
    {
        _uiInventory = uiInventory;
        // 인벤토리 타입에 따른 Action 매핑
        _OnClickActions[EInventoryType.Inventory] = OnInventoryClick;
        _OnClickActions[EInventoryType.Craft] = OnCraftClick;
        _OnClickActions[EInventoryType.Equipment] = OnEquipmentClick;
    }

    protected override void UpdateUI(InventoryItem data)
    {
        if (data?.itemData == null)
        {
            ClearUI(); 
            return;
        }

        if (_btn != null)
        {
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(OnClick);
        }
        if (_countTxt != null) _countTxt.text = data.Amount.ToString();
        if (_icon != null) _icon.sprite = data.GetSprite();
        _icon.enabled = data != null;
        _imageObject = _icon.gameObject;
        _textObject = _countTxt.gameObject;
        ShowIcon();
        ShowText();
    }

    protected override void ClearUI()
    {
        if (_icon == null || _icon.gameObject == null) return;
        _icon.sprite = null;
        _countTxt.text = string.Empty;
        _icon.enabled = false;

        if (_btn != null) _btn.onClick.RemoveAllListeners();

        _data = null;

        if (_imageObject != null) HideIcon();
        if (_textObject != null) HideText();
    }

    protected override void ShowTooltip(InventoryItem data)
    {
        if (data == null || data.itemData == null) return;

        UIManager.Show<UI_ItemTooltip>(data.itemData);
    }

    protected override void HideTooltip()
    {
         UIManager.Hide<UI_ItemTooltip>();
    }
    public override void OnClick()
    {
        if (!HasData) return;

        var item = _data;
        HideTooltip();
        //인벤토리타입에 맞는 액션 찾기
        if (_OnClickActions.TryGetValue(_uiInventory.CurType, out var action))
        {
            //찾았다면 Invoke
            action.Invoke();
        }
        else
        {
        }
        //UIManager.Show<UI_Action>((int)_uiInventory.CurType, transform as RectTransform, _data, Index);
    }

    private void OnInventoryClick()
    {
        var item = _data;
        // Consumable 타입 검사
        //if (item.GetItemType() != Item_Type.Consumable) return;
        UIManager.Show<UI_Action>((int)_uiInventory.CurType, _rectTransform, _data, Index);
    }

    private void OnCraftClick()
    {
        if (DragManager.Instance.IsDragging) return;
        Inventory.Instance.Craft(_data);
    }

    private void OnEquipmentClick()
    {
        // 예를 들면 장비 착용창 열기
        UIManager.Show<UI_Action>((int)_uiInventory.CurType, _rectTransform, _data, Index);
    }

    public void ReduceItem(int amount = 1) // 아이템 수량감소
    {
        if(int.TryParse(_countTxt.text, out int i))
        {
            i -= amount;
            _countTxt.text = i.ToString();
        }
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
        //_itemSprite.color = _blurColor; 
        _icon.color = _blurColor; 
    }
    public void SetItemNormalColor()
    {
        //_itemSprite.color = _normalBorderColor;
        _icon.color = _normalBorderColor;
    }
    public void SetItemopaque()
    {
        //_itemSprite.color = _opaqueColor;
        _icon.color = _opaqueColor;
    }

    public void SetHighlight(bool isOn)
    {
        if (_borderImage == null)
        {
            return;
        }

        if (isOn)
        {
        }
        else
        {
            SetNormalColor();
        }
    }

    public InventoryItem GetItem()
    {
        return _data;
    }
}
