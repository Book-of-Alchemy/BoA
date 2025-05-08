using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confused : Debuff
{
    public Confused(StatusEffectData data)
    {
        this.data = data;
    }
    public Confused(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        if (target is PlayerStats player)
        {
            var behavior = player.GetComponent<DungeonBehavior>();
            if (behavior != null)
            {
                //behavior.canMove = false;
            }
        }
        else if (target is EnemyStats enemy)
        {
            var controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.isConfused = true;
            }
        }
    }

    public override void OnExpire(CharacterStats target)
    {
        if (target is PlayerStats player)
        {
            var behavior = player.GetComponent<DungeonBehavior>();
            if (behavior != null)
            {
                behavior.canMove = true;
            }
        }
        else if (target is EnemyStats enemy)
        {
            var controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.isConfused = false;
            }
        }
    }
}
