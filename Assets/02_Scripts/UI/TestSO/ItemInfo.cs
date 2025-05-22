using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _itemNameTxt;
    [SerializeField] private TextMeshProUGUI _itemDescTxt;
    [SerializeField] private TextMeshProUGUI _itemTagTxt;
    [SerializeField] private Image _image;
    [SerializeField] private Image _rangeImage;
    [SerializeField] private Image _effectImage;

    private void Awake()
    {
        ClearInfo();
    }

    public void ClearInfo() //모든 정보 null
    {
        _itemNameTxt.text = null;
        _itemDescTxt.text = null;
        _itemTagTxt.text = null;
        _image.sprite = null;
        _rangeImage.sprite = null;
        _effectImage.sprite = null;
        _image.gameObject.SetActive(false);
        _rangeImage.gameObject.SetActive(false);
        _effectImage.gameObject.SetActive(false);
    }

    public void ShowInfo(InventoryItem item) // 인자 값에 정보를 UI에 보여줌
    {
        _itemNameTxt.text = item.GetItemName();
        _itemDescTxt.text = item.GetItemDesc();
        if(item.itemData.tags !=null)
            _itemTagTxt.text = string.Join(", ", item.itemData.tags.Select(tag => tag.ToString()));
        _image.sprite = item.GetSprite();
        if (item.itemData.itemRangeSprite != null)
        {
            _rangeImage.sprite = item.itemData.itemRangeSprite;
            _rangeImage.gameObject.SetActive(true);
        }
        if (item.itemData.itemEffectRangeSprite != null)
        {
            _effectImage.sprite = item.itemData.itemEffectRangeSprite;
            _effectImage.gameObject.SetActive(true);
        }
        _image.gameObject.SetActive(true);
    }
}