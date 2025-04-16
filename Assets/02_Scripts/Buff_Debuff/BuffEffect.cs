using UnityEngine;
using UnityEngine.InputSystem.Layouts;

public enum BuffEffectType
{
    AttackBuff,//공격증가
    DefenseBuff,//방어증가
    SpeedBuff,//한턴에 두번 행동가능
    Regeneration,//재생
    Shield,//보호막
    Flight,//비행
    // ===================================================
    AttackDebuff,//공격력 감소
    DefenseDebuff,//방어력 감소
    Burn,//화상
    Poison,//독
    Bleeding,//출혈
    Shock,//감정
    Blind,//실명
    Confusion,//혼란
    Root,   //두턴에 한번 행동가능
    Stun, //이동, 공격 다 불가능
    Sleep,//스턴과 같은데 공격당하면 풀림
    //=====================================================
    // 면역 버프 (상태이상 면역)
    BurnImmunity,
    PoisonImmunity,
    StunImmunity,
    SleepImmunity,
    RootImmunity,
    ConfusionImmunity,
    BlindImmunity,
    BleedingImmunity

}

[System.Serializable]
public class BuffEffect
{
    public BuffEffectType effectType_;//효과타입
    public Sprite icon_;//아이콘
    public float value_;//효과 값(액션포인트 보정치)
    public int duration_;//지속턴수
    public bool stackable_;//중첩가능여부

    public BuffEffect(BuffEffectType effectType, Sprite icon, float value, int duration, bool stackable)
    {
        effectType_ = effectType;
        icon_ = icon;
        value_ = value;

    }


}
