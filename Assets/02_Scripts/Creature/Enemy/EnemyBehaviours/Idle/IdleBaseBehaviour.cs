using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBaseBehaviour : BaseBehaviour
{
    public override void Excute()
    {
        if (StateCheck())
            Action();
    }

    public override bool StateCheck()
    {
        foreach (Tile tile in vision)
        {
            if (tile.characterStats is PlayerStats player)
            {
                controller.ChangeState(EnemyState.Chase);
                controller.lastCheckedTile = tile;
                return false;
            }
        }

        return true;
    }

    public override void Action()
    {
        
    }
}
