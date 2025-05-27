
public class LavaTile : TileEffect, IGround,IWater
{
    public override EnvironmentType EnvType => EnvironmentType.Lava;
    public override void PerformAction()
    {
        if(CurTile.CharacterStatsOnTile != null)
        {
            var target = CurTile.CharacterStatsOnTile;
            DamageInfo damage = new DamageInfo(target.MaxHealth * 0.1f,DamageType.Fire ,null, target);
            target.TakeDamage(damage);
            TileRuleProccessor.ProcessTileReactions(damage, CurTile);
        }

        StatusEffectFactory.CreateEffect(220009, CurTile.CharacterStatsOnTile);
    }

}