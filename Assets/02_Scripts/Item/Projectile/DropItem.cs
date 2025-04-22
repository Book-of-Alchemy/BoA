using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class DropItem : MonoBehaviour
{
    public BaseItem item;
    public ItemData itemData;
    public SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    public DropObject dropObject;


    public void Init(ItemData data, BaseItem choiceItem)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dropObject = GetComponent<DropObject>();
        item = choiceItem;
        SetType(data);
        
    }

    private void SetType(ItemData data)
    {
        itemData = data;
        spriteRenderer.sprite = data.Sprite;
    }

    //private void SetEffectType(Effect_Type type)
    //{
    //    switch (type)
    //    {
    //        case Effect_Type.Damage:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<DamageItem>(); break;
    //        case Effect_Type.Heal:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<HealItem>(); break;
    //        case Effect_Type.Buff:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<BuffItem>(); break;
    //        case Effect_Type.Debuff:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<DeBuffItem>(); break;
    //        case Effect_Type.Move:
    //            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<MoveItem>(); break;
    //    }
    //}


}
