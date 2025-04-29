using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChaseBehaviour : ChaseBaseBehaviour
{
    public override void Action()
    {
        List<Tile> path = AStarPathfinder.FindPathForCreature(CurTile, controller.LastCheckedTile, level);
        if (path == null)
            return;
        if (path.Count > 15 || path.Count <= 1)
            return;

        MoveTo(path[1]);
    }

}
