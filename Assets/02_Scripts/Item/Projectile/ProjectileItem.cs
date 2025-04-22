using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ProjectileItem : MonoBehaviour
{
    public BaseItem item;
    public ItemData itemData;
    public SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    public ProjectileMove projectileMove;



    public void Init(ItemData data, BaseItem choiceItem)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        projectileMove = GetComponent<ProjectileMove>();
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

    // TragetRange가 0인것은 플레이어로부터 시작되는 공격
    // 공격시 아이템을 던질 위치를 받고 그 범위에 해당하는 몬스터 리스트를 받아옴
    // 받아온 리스트를 UseItem에 넣고 로직 처리
    // TragetRange가 0인것은 플레이어를 제외한 나머지 범위의 몬스터리스트를 받아와야한다.

}
