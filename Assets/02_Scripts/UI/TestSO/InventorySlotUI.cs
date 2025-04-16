using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _countTxt;
    [SerializeField] private Button _btn;

    public int Index { get; private set; }
    public bool HasItem => _itemSprite.sprite != null; //있다면 True

    public void UpdateInventorySlot(InventoryItemData item) //슬롯 UI 갱신
    {
        _itemSprite.sprite = item.Icon;
        //_numTxt = item.
    }

    public void ClearInventorySlot() 
    {
        _itemSprite = null;
        _countTxt = null; 
    }

    public void SetItem(Sprite sprite) // 슬롯에 아이템 등록
    {
        if(sprite != null)
        {
            _itemSprite.sprite = sprite;

        }
        else
        {

        }
    }
    public void SetItemAmount(int amount)
    {

        _countTxt.text = amount.ToString();
    }

    public void RemoveItem() // 슬롯에서 아이템제거
    {
        _itemSprite.sprite = null;
    }

}
