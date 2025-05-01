using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ChaseBaseBehaviour : BaseBehaviour
{
    public override int ActionCost => 10;
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
                    controller.ChangeState(EnemyState.Attack);
                    return false;
                }
                break;
            }
        }

        foreach (Tile tile in vision)//플레이어가 시야에 있다면 lastchecked에 넣기
        {
            if (tile.CharacterStatsOnTile is PlayerStats player)
            {
                controller.LastCheckedTile = tile;
                break;
            }
        }

        if (controller.LastCheckedTile == null)//플레이어가 시야에 없다면 마지막으로 보인곳으로 감 마지막으로 보인곳에 도달하면 다시 idle
        {
            controller.ChangeState(EnemyState.Idle);
            return false;
        }

        return true;
    }

    public override void Action()
    {

    }

    protected void MoveTo(Tile target)
    {
        if (target.CharacterStatsOnTile != null)//character 존재시 이동불가
            return;

        controller.MoveTo(
            target.gridPosition,
            () =>
            {
                CurTile.CharacterStatsOnTile = null;
                CurTile = target;
                CurTile.CharacterStatsOnTile = enemyStats;

                if (target == controller.LastCheckedTile)
                    controller.LastCheckedTile = null;
                EndTurn();
            }
        );

    }
}
