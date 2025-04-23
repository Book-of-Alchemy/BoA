using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _itemNameTxt;
    [SerializeField] private TextMeshProUGUI _itemDescTxt;
    [SerializeField] private Image _image;

    private void Awake()
    {
        ClearInfo();
    }

    public void ClearInfo() //모든 정보 null
    {
        _itemNameTxt.text = null;
        _itemDescTxt.text = null;
        _image.sprite = null;
        _image.gameObject.SetActive(false);
    }

    public void ShowInfo(InventoryItem item) // 인자 값에 정보를 UI에 보여줌
    {
        _itemNameTxt.text = item.GetItemName();
        _itemDescTxt.text = item.GetItemDesc();
        _image.sprite = item.GetSprite();
        _image.gameObject.SetActive(true);
    }
}