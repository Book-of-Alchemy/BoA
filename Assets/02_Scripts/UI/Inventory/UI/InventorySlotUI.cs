using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.Mathematics;

public class InventorySlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _countTxt;
    [SerializeField] private Button _btn;
    [SerializeField] private RectTransform _rectTransform;

    private GameObject _imageObject;
    private GameObject _textObject;
    public int Index { get; set; }
    public bool HasItem => _itemSprite.sprite != null; //있다면 True

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

        _btn.onClick.AddListener(OnClickItem);

        HideIcon();
    }

    public void OnClickItem() // Call at OnClick Event
    {
        UIManager.Show<UI_Action>(eUIActionType.Use,_rectTransform);
    }

    public void SetItem(InventoryItem item) // 슬롯에 아이템 등록
    {
        _itemSprite.sprite = item.GetSprite();
        _countTxt.text = item.Amount.ToString();
        ShowIcon();
    }

    public void RemoveItem() // 슬롯에서 아이템제거
    {
        _itemSprite.sprite = null;
        _countTxt.text = null;
        HideIcon();
        HideText();
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        OnSelected?.Invoke(Index);
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        OnDeselected?.Invoke(Index);
        if(UIManager.Get<UI_Action>() != null)
            UIManager.Hide<UI_Action>();
    }
}
