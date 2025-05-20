using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreSummon : EnemySkill
{
    public override bool CanUse()//캔유즈 실행후 enemy 애니메이션 실행 예정
    {
        return false;
    }
    public override void Use() //스킬 애니메이션 중간에 실행 예정
    {

    }

    /// <summary>
    /// 실질적인 대미지를 주는 메서드 
    /// 원거리 공격시 use에서 이펙트프로젝타일매니저 launch후 콜백액션으로 달아주며 
    /// 근거리공격시 use내에서 dealdamage처리하도록
    /// </summary>
    public override void DealDamage()
    {

    }
}