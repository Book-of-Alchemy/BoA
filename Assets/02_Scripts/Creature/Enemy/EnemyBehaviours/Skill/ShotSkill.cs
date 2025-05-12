using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ShotSkill : EnemySkill
{
    List<Tile> neighbors => attackBaseBehaviour.adjacentiveTile;
    List<Tile> attackRange => attackBaseBehaviour.attackRangeTile;
    public override bool CanUse()
    {
        
        foreach (Tile tile in neighbors)
        {
            if (tile.CharacterStatsOnTile is PlayerStats player)
            {
                if(player.IsHidden)
                    continue;
                return false;
            }
        }

        foreach (Tile tile in attackRange)
        {
            if (tile.CharacterStatsOnTile is PlayerStats player)
            {
                if (player.IsHidden)
                    continue;
                attackBaseBehaviour.playetStats = player;
                return true;
            }
        }

        return false;
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
        stats.Attack(attackBaseBehaviour.playetStats, 2f);
        attackBaseBehaviour.EndTurn();
    }
}
