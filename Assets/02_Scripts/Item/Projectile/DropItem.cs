using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public BaseItem Item;
    public SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    public DropObject dropObject;


    public void Init(ItemData data)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dropObject = GetComponent<DropObject>();
        SetType(data);
    }

    private void SetType(ItemData data)
    {
        Item = ResourceManager.Instance.effectTypeData[data.effect_type];
        Item.data = data;
        spriteRenderer.sprite = Resources.Load<Sprite>(Item.data.icon_sprite);
    }
}
