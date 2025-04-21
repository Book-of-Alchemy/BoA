using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChaseBehaviour : ChaseBaseBehaviour
{
    public override void Action()
    {
        List<Tile> path = AStarPathfinder.FindPath(CurTile, controller.LastCheckedTile, level);
        if (path == null || path.Count <= 1)
            return;

        MoveTo(path[1]);
    }

}
