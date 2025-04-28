using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBaseBehaviour : BaseBehaviour
{
    public override int ActionCost => 10;
    public EnemySkill skill_0;
    public EnemySkill skill_1;
    protected PlayerStats playetStats;
    public override void Excute()
    {
        if (StateCheck())
            Action();
    }

    public override bool StateCheck()
    {
        foreach (Tile tile in attackRangeTile)//공격 사거리 안에 있으며 시야에 있는지 체크
        {
            if (tile.CharacterStatsOnTile is PlayerStats player)
            {
                if (TileUtility.IsTileVisible(level, CurTile, tile))
                {
                    playetStats = player;
                    return true;
                }
                break;
            }
        }

        playetStats = null;
        controller.ChangeState(EnemyState.Chase);
        return false;
    }

    public override void Action()
    {
        
    }
}