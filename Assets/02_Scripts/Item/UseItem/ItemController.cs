using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public ItemData data;
    public BaseItem Item = new BaseItem();
    public Image sptrite; // 투사체를 아이템이미지로 바꿀이미지


    public void Init(ItemData data)
    {
        GetData(data);
        if (this.data != null) Item.Init(this.data);
    }
    

    /// <summary>
    /// 투사체 프리팹에서 데이터 불러옴
    /// </summary>
    /// <param name="data"></param>
    public void GetData(ItemData data)
    {
        this.data = data;
    }

}
