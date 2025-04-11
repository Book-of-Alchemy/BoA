using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : UIBase
{
    public GameObject prefab;

    public List<Slots> slots;
    private int slotCount;

    public List<TestItem> test;

    private void Start()
    {
   
    }

    public override void HideDirect() //Call at OnClick Event 
    {
        UIManager.Hide<UI_Inventory>();
    }

    public override void Opened(params object[] param)
    {
        
    }

    public void OnClickAddItem()
    {
        if(slotCount < slots.Count)
        {
            SlotItem item = UIResourceManager.Instance.LoadAsset<SlotItem>();
            item.Init(test[Random.Range(0, test.Count)]);
            UIResourceManager.Instance.InstantiateAsset<SlotItem>(slots[slotCount++].transform);
        }
    }
}
