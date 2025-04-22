using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileItem : MonoBehaviour
{
    public BaseItem Item;
    public SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    public ProjectileMove projectileMove;
    //DamageItem damageItem;
    //HealItem healItem;
    //BuffItem buffItem;
    //DeBuffItem deBuffItem;
    //MoveItem moveItem;


    public void Init(ItemData data)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        projectileMove = GetComponent<ProjectileMove>();
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
            Item = Instantiate(ResourceManager.Instance.typeObjectPrefab,this.transform).GetComponent<DamageItem>(); break;
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

    // TragetRange가 0인것은 플레이어로부터 시작되는 공격
    // 공격시 아이템을 던질 위치를 받고 그 범위에 해당하는 몬스터 리스트를 받아옴
    // 받아온 리스트를 UseItem에 넣고 로직 처리
    // TragetRange가 0인것은 플레이어를 제외한 나머지 범위의 몬스터리스트를 받아와야한다.

}
