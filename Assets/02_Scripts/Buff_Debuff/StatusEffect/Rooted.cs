using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooted : Debuff
{
    public Rooted(StatusEffectData data)
    {
        this.data = data;
    }
    public Rooted(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (target is PlayerStats player)
        {
            var behavior = player.GetComponent<DungeonBehavior>();
            if (behavior != null)
            {
                behavior.canMove = false;
            }
        }
        else if (target is EnemyStats enemy)
        {
            var controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.canMove = false; 
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
                controller.canMove = true; 
            }
        }
    }
}
