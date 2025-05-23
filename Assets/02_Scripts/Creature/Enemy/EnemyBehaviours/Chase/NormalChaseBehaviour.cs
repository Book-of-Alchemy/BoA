using System.Collections;
using System.Collections.Generic;


public class NormalChaseBehaviour : ChaseBaseBehaviour
{
    public override void Action()
    {
        List<Tile> path = AStarPathfinder.FindPathForCreature(CurTile, controller.LastCheckedTile, level);
        if (path == null)
        {
            EndTurn();
            return;
        }
        if (path.Count > 15 || path.Count <= 1)
        {
            EndTurn();
            return;
        }

        MoveTo(path[1]);
    }

}
