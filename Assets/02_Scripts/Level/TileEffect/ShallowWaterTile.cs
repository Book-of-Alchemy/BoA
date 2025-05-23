
using System.Linq;

public class ShallowWaterTile : TileEffect, IGround, IWater
{
    public override EnvironmentType EnvType => EnvironmentType.Shallow_Water;
    public override void PerformAction()
    {
        Burn burn = null;
        if (CurTile.CharacterStatsOnTile != null)
        {
            burn = CurTile.CharacterStatsOnTile.activeEffects
                .OfType<Burn>()
                .FirstOrDefault();
        }

        if (burn != null)
        {
            burn.Cleanse();
        }
    }
}
