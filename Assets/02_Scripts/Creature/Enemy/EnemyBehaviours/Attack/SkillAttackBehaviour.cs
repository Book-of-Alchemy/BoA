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
            if (skill_0.CanUse())
            {
                controller.curSkill = skill_0;
                controller.Skill_0();
            }
        }

        if (skill_1 != null)
        {
            if (skill_1.CanUse())
            {
                controller.curSkill = skill_1;
                controller.Skill_1();
            }
        }

        controller.Attack();
    }
}
