using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public ItemData data;
    public BaseItem Item;
    public Image sptrite; // 투사체를 아이템이미지로 바꿀이미지


    public void Init(ItemData data)
    {
        this.data = data;
        if (this.data != null) SetType();
    }

    private void SetType()
    {
        Item = ResourceManager.Instance.effectTypeData[data.effect_type];
        Item.Init(data);
    }
}
