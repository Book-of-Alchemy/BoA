

public class IdleBaseBehaviour : BaseBehaviour
{
    public override float ActionCostMultiPlier => 1f;
    public override void Excute()
    {
        if (StateCheck())
            Action();
    }

    public override bool StateCheck()
    {
        foreach (Tile tile in vision)
        {
            if (tile.CharacterStatsOnTile is PlayerStats player)
            {
                if (player.IsHidden)
                    continue;
                controller.ChangeState(EnemyState.Chase);
                controller.LastCheckedTile = tile;
                return false;
            }
        }

        return true;
    }

    public override void Action()
    {
        
    }
}
