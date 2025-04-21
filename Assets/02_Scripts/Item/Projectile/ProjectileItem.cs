using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class  ProjectileItem : MonoBehaviour
{
    public BaseItem Item;
    public SpriteRenderer spriteRenderer; // 투사체를 아이템이미지로 바꿀이미지
    public ProjectileMove projectileMove;


    public void Init(ItemData data)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        projectileMove = GetComponent<ProjectileMove>();
        SetType(data);
    }

    private void SetType(ItemData data)
    {
        Item = ResourceManager.Instance.effectTypeData[data.effect_type];
        Item.data = data;
        spriteRenderer.sprite = Resources.Load<Sprite>(Item.data.icon_sprite);
    }

    // TragetRange가 0인것은 플레이어로부터 시작되는 공격
    // 공격시 아이템을 던질 위치를 받고 그 범위에 해당하는 몬스터 리스트를 받아옴
    // 받아온 리스트를 UseItem에 넣고 로직 처리
    // TragetRange가 0인것은 플레이어를 제외한 나머지 범위의 몬스터리스트를 받아와야한다.

}
