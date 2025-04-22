using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public BaseItem Item;
    public SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    public DropObject dropObject;
    //DamageItem damageItem;
    //HealItem healItem;
    //BuffItem buffItem;
    //DeBuffItem deBuffItem;
    //MoveItem moveItem;


    public void Init(ItemData data)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dropObject = GetComponent<DropObject>();
        //damageItem = GetComponent<DamageItem>();
        //healItem = GetComponent<HealItem>();
        //buffItem = GetComponent<BuffItem>();
        //deBuffItem = GetComponent<DeBuffItem>();
        //moveItem = GetComponent<MoveItem>();
        SetType(data);
    }

    private void SetType(ItemData data)
    {
        SetEffectType(data.effect_type);
        Item.data = data;
        spriteRenderer.sprite = Resources.Load<Sprite>(Item.data.icon_sprite);
    }

    private void SetEffectType(Effect_Type type)
    {
        switch (type)
        {
            case Effect_Type.Damage:
                Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<DamageItem>(); break;
            case Effect_Type.Heal:
                Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<HealItem>(); break;
            case Effect_Type.Buff:
                Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<BuffItem>(); break;
            case Effect_Type.Debuff:
                Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<DeBuffItem>(); break;
            case Effect_Type.Move:
                Item = Instantiate(ResourceManager.Instance.typeObjectPrefab, this.transform).GetComponent<MoveItem>(); break;
        }
    }


}
