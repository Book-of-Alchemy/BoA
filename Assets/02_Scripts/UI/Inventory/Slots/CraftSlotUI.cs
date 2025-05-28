using UnityEngine;

public class CraftSlotUI : InventorySlotUI
{
    public override void SetData(InventoryItem item)
    {
        base.SetData(item); 
        _imageObject.SetActive(true);
        _textObject.SetActive(true);
    }

    protected override void ClearUI()
    {
        _imageObject.SetActive(false);
        _textObject.SetActive(false);
        _icon.sprite = null;
        _countTxt.text = string.Empty;
        if (_btn != null) _btn.onClick.RemoveAllListeners();
    }

    protected override void UpdateUI(InventoryItem data)
    {
        if (data?.itemData?.sprite == null) return;
        _btn.onClick.RemoveAllListeners();
        _btn.onClick.AddListener(OnClick);
        _icon.sprite = data.itemData.sprite;
        _icon.enabled = true;

        _countTxt.text = data.Amount.ToString();
    }


    public override void OnClick()
    {
        //if (!HasData) return;

        //Inventory.Instance.RemoveCraftTable(_data);
        if (!HasData)
        {
            Debug.Log("클릭: 데이터 없음");
            return;
        }

        
        Inventory.Instance.RemoveCraftTable(_data);
        RemoveData();
    }

}
