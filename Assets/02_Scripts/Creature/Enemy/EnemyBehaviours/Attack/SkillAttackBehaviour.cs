using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackBehaviour : AttackBaseBehaviour
{
    public override void Action()
    {
        if (skill_0 != null)
        {
            var result = skill_0.CanUse();

            if (result == SkillUseState.ReadyToCast)
            {
                controller.curSkill = skill_0;
                controller.Skill_0();
                return;
            }
            else if (result == SkillUseState.Preparing)
            {
                controller.EndTurn(); // 준비 턴 종료
                return;
            }
        }

        if (skill_1 != null)
        {
            var result = skill_1.CanUse();

            if (result == SkillUseState.ReadyToCast)
            {
                controller.curSkill = skill_1;
                controller.Skill_1();
                return;
            }
            else if (result == SkillUseState.Preparing)
            {
                controller.EndTurn(); // 준비 턴 종료
                return;
            }
        }

        controller.Attack(); // 스킬 준비도 못 했으면 일반 공격
    }
    //public override void Action()
    //{
    //    if (skill_0 != null)
    //    {
    //        if (skill_0.CanUse())
    //        {
    //            controller.curSkill = skill_0;
    //            controller.Skill_0();
    //            return;
    //        }
    //    }

    //    if (skill_1 != null)
    //    {
    //        if (skill_1.CanUse())
    //        {
    //            controller.curSkill = skill_1;
    //            controller.Skill_1();
    //            return;
    //        }
    //    }

    //    controller.Attack();
    //}
}
