
public class SlimedFieldTile : TileEffect, IGround, IWater
{
    public override EnvironmentType EnvType => EnvironmentType.Slimed_Field;
    public override void PerformAction()
    {
        StatusEffectFactory.CreateEffect(220006, CurTile.CharacterStatsOnTile);
        StatusEffectFactory.CreateEffect(220010, CurTile.CharacterStatsOnTile);
    }
}

