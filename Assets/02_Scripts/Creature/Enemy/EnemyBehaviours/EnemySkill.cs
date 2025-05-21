using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum SkillUseState
{
    CannotUse,       // 조건 미달
    Preparing,       // 준비단계 (이번 턴에 애니메이션 안 나감)
    ReadyToCast      // 준비완료 → 애니메이션 실행 가능
}

public abstract class EnemySkill : MonoBehaviour
{
    protected EnemyStats stats;
    protected AttackBaseBehaviour attackBaseBehaviour;
    protected virtual void Awake()
    {
        stats = GetComponent<EnemyStats>();
        attackBaseBehaviour = GetComponent<AttackBaseBehaviour>();
    }

    public abstract SkillUseState CanUse();//캔유즈 실행후 enemy 애니메이션 실행 예정
    public abstract void Use(); //스킬 애니메이션 중간에 실행 예정

    /// <summary>
    /// 실질적인 대미지를 주는 메서드 
    /// 원거리 공격시 use에서 이펙트프로젝타일매니저 launch후 콜백액션으로 달아주며 
    /// 근거리공격시 use내에서 dealdamage처리하도록
    /// </summary>
    public abstract void DealDamage();
}
