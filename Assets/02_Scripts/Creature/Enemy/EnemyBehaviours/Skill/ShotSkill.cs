using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ShotSkill : EnemySkill
{
    List<Tile> neighbors => attackBaseBehaviour.adjacentiveTile;
    List<Tile> attackRange => attackBaseBehaviour.attackRangeTile;
    public override SkillUseState CanUse()
    {
        
        foreach (Tile tile in neighbors)
        {
            if (tile.CharacterStatsOnTile is PlayerStats player)
            {
                if(player.IsHidden)
                    continue;
                return SkillUseState.CannotUse;
            }
        }

        foreach (Tile tile in attackRange)
        {
            if (tile.CharacterStatsOnTile is PlayerStats player)
            {
                if (player.IsHidden)
                    continue;
                attackBaseBehaviour.playetStats = player;
                return SkillUseState.ReadyToCast;
            }
        }

        return SkillUseState.CannotUse;
    }
    public override void Use()
    {
        EffectProjectileManager.Instance.LaunchProjectile
            (
            stats.CurTile.gridPosition,
            attackBaseBehaviour.playetStats.CurTile.gridPosition,
            310000,
            DealDamage
            );
    }

    public override void DealDamage()
    {
        SoundManager.Instance.Play("pierce");
        stats.Attack(attackBaseBehaviour.playetStats, 2f);
        attackBaseBehaviour.EndTurn();
    }
}
