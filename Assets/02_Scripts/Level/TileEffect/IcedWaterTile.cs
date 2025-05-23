
public class IcedWaterTile : TileEffect, IGround, IWater
{
    public override EnvironmentType EnvType => EnvironmentType.Iced_Water;
    public override void PerformAction()
    {
        StatusEffectFactory.CreateEffect(220006, CurTile.CharacterStatsOnTile);
    }
}