using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct TileReactionResult
{
    public bool isReacted;
    public EnvironmentType resultTileType;
    public int effect_ID;
    public float damage;
    public DamageType damageType;

    public TileReactionResult(bool isReacted, EnvironmentType resultTileType, int effect_ID, float damage, DamageType damageType)
    {
        this.isReacted = isReacted;
        this.resultTileType = resultTileType;
        this.effect_ID = effect_ID;
        this.damage = damage;
        this.damageType = damageType;
    }

    // 실패
    public static TileReactionResult NoReaction => new TileReactionResult(false, EnvironmentType.none,-1, 0f, DamageType.None);
}
public static class TileRuleProccessor
{
    private static Dictionary<(DamageType, EnvironmentType), EnvRuleData> ruleDic => SODataManager.Instance.envRuleDataBase.ruleDic;
    public static TileReactionResult CheckRuleOnGround(DamageInfo damageInfo, Tile tile)
    {

        if (!ruleDic.ContainsKey((damageInfo.damageType, tile.groundEffect.EnvType)))
            return TileReactionResult.NoReaction;

        EnvRuleData rule = ruleDic[(damageInfo.damageType, tile.groundEffect.EnvType)];
        bool isReacted = true;
        EnvironmentType resultTileType = rule.resultTileType;
        int effect_ID = rule.effect_ID;
        float damage = rule.damageMultiplier * damageInfo.value;
        DamageType damageType = damageInfo.damageType;

        TileReactionResult result = new TileReactionResult(isReacted, resultTileType, effect_ID, damage, damageType);
        return TileReactionResult.NoReaction;
    }
    public static TileReactionResult CheckRuleOnAir(DamageInfo damageInfo, Tile tile)
    {
        if (!ruleDic.ContainsKey((damageInfo.damageType, tile.airEffect.EnvType)))
            return TileReactionResult.NoReaction;

        EnvRuleData rule = ruleDic[(damageInfo.damageType, tile.groundEffect.EnvType)];
        bool isReacted = true;
        EnvironmentType resultTileType = rule.resultTileType;
        int effect_ID = rule.effect_ID;
        float damage = rule.damageMultiplier * damageInfo.value;
        DamageType damageType = damageInfo.damageType;

        TileReactionResult result = new TileReactionResult(isReacted, resultTileType, effect_ID, damage, damageType);
        return TileReactionResult.NoReaction;
    }


}
