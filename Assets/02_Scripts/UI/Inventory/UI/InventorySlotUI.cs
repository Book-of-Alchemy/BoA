using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _countTxt;
    [SerializeField] private Button _btn;

    private GameObject _imageObject;
    private GameObject _textObject;
    public int Index { get; private set; }
    public bool HasItem => _itemSprite.sprite != null; //있다면 True

    private void ShowIcon() => _imageObject.SetActive(true);
    private void HideIcon() => _imageObject.SetActive(false);
    private void ShowText() => _textObject.SetActive(true);
    private void HideText() => _textObject.SetActive(false);

    private void Awake()
    {
        _imageObject = _itemSprite.gameObject;
        _textObject = _countTxt.gameObject;
        HideIcon();
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

}
