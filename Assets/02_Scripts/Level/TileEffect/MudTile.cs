
public class MudTile : TileEffect, IGround, IWater
{
    public override EnvironmentType EnvType => EnvironmentType.Mud;
    public override void PerformAction()
    {
        StatusEffectFactory.CreateEffect(220006, CurTile.CharacterStatsOnTile);
    }
}
