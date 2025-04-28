using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;
using static UnityEditor.Progress;

public class InventorySlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler
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

    public int Index { get; set; }
    public bool HasItem => _itemSprite.sprite != null; //있다면 True

    private Color _normalBorderColor = new Color(1f, 1f, 1f, 0f); // 기본(투명)
    private Color _highlightBorderColor = new Color(1f, 1f, 0f, 1f); // 강조(노란색)

    private void ShowIcon() => _imageObject.SetActive(true);
    private void HideIcon() => _imageObject.SetActive(false);
    private void ShowText() => _textObject.SetActive(true);
    private void HideText() => _textObject.SetActive(false);

    public event Action<int> OnSelected;
    public event Action<int> OnDeselected;

    private void Awake()
    {
        _imageObject = _itemSprite.gameObject;
        _textObject = _countTxt.gameObject;
        if (_borderImage != null)
            _borderImage.color = _normalBorderColor;
        HideIcon();
    }

    public virtual void OnClickItem() // Call at OnClick Event
    {
        if(HasItem) // 아이템 있을때만
        {
            _uiInventory.SetSelectIndex(Index);
            UIManager.Show<UI_Action>((int)_uiInventory.curType, _rectTransform,_item,Index);
        }
    }

    public void SetItem(InventoryItem item) // 슬롯에 아이템 등록
    {
        _item = item;
        if (!HasItem)
            _btn.onClick.AddListener(OnClickItem);
        _countTxt.text = _item.Amount.ToString();
        _itemSprite.sprite = _item.GetSprite();
        ShowIcon();
        ShowText();
    }

    public void RemoveItem() // 슬롯에서 아이템제거
    {
        _btn.onClick.RemoveListener(OnClickItem);
        _itemSprite.sprite = null;
        _countTxt.text = null;
        _item = null;
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
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        OnDeselected?.Invoke(Index);
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
            _borderImage.color = _highlightBorderColor; // 강조 색상 적용
        }
        else
        {
            _borderImage.color = _normalBorderColor; // 기본 색상 복귀
        }
    }
}
