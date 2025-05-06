using UnityEngine;

public enum BuffEffectType
{
    // 버프 효과
    AttackIncrease,    // 공격력 증가 (중첩 불가)
    DefenseIncrease,   // 방어력 증가 (중첩 불가)
    SpeedIncrease,     // 신속 (한 턴에 두 번 행동 가능; 둔화 반대)
    Regeneration,      // 재생 (매 턴 HP % 회복)
    Shield,            // 보호막 (일정 피해 흡수)
    Flight,            // 비행 (함정 트리거 X, 일부 위험 지형 효과 X)

    // 디버프 효과
    AttackDecrease,    // 공격력 감소 (중첩 불가)
    DefenseDecrease,   // 방어력 감소 (중첩 불가)
    Burn,              // 화상 (매 턴 피해, 해제 조건 존재)
    Poison,            // 중독 (매 턴 피해; 스택 가능)
    Bleeding,          // 출혈 (이동 시 스택 증가, 공격 갱신 시 피해 증가)
    Shock,             // 감전 (피격 시 추가 피해)
    Blind,             // 실명 (플레이어 인식 X)
    Confusion,         // 혼란 (무작위 방향 이동/공격)
    Root,              // 속박 (이동 불가; 공격 가능)
    Stun,              // 기절 (이동, 공격 모두 불가)
    Sleep,             // 수면 (행동 불가, 공격 시 해제)

    // 면역 버프 효과 (각 디버프에 대해 개별 면역 제공)
    BurnImmunity,      // 화상 면역
    PoisonImmunity,    // 중독 면역
    StunImmunity,      // 기절 면역
    SleepImmunity,     // 수면 면역
    RootImmunity,      // 속박 면역
    ConfusionImmunity, // 혼란 면역
    BlindImmunity,     // 실명 면역
    BleedingImmunity   // 출혈 면역
}

public enum StatusType
{
    buff,
    debuff
}
public enum EffectCategory
{
    stat_down,
    action_block,
    dot,
    stat_up,
    immunity,
    extra_action,
    sustain,
}

public enum DurationType
{
    turn,
    damage_absorb
}

[CreateAssetMenu(fileName = "StatusEffectInfo", menuName = "StatusEffect/StatusEffect")]
public class StatusEffectData : ScriptableObject
{
    public int id;
    public string name_kr;
    public StatusType type;
    public EffectCategory effect_category;
    public string icon_sprite;
    public string description;
    public DurationType duration_type;
    public bool isStackable;
    public string special_note;
    public Sprite icon;
}