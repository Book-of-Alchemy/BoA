
public class FogTile : TileEffect, IAir, IExpirable
{
    public override EnvironmentType EnvType => EnvironmentType.Fog;
    private int leftTime = 50;
    public int LeftTime { get => leftTime; set => leftTime = value; }

    public override void Init(Tile tile)
    {
        base.Init(tile);
        TileUtility.RefreshLevelSight();
    }
    public override void PerformAction()
    {
        CurTile.RefreshSight();
        if (LeftTime <= 0)
        {
            Expire();
            return;
        }

        LeftTime -= ActionCost;
    }

    public void Expire()
    {
        EnvironmentalFactory.Instance.ReturnTileEffect(this);
    }
}

