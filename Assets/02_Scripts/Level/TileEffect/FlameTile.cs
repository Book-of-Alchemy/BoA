

public class FlameTile : TileEffect, IGround,IExpirable
{
    public override EnvironmentType EnvType => EnvironmentType.Flame;
    private int leftTime = 50;
    public int LeftTime {  get => leftTime; set => leftTime = value; }
    public override void PerformAction()
    {
        if(LeftTime <= 0)
        {
            Expire();
            return;
        }

        StatusEffectFactory.CreateEffect(220009, CurTile.CharacterStatsOnTile);
        LeftTime -= ActionCost;
    }

    public void Expire()
    {
        EnvironmentalFactory.Instance.ReturnTileEffect(this);
    }
}

