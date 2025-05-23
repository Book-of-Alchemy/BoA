using System.Collections.Generic;
using UnityEngine;
public struct TileReactionResult
{
    public bool isReacted;
    public EnvironmentType sourceTileType;
    public EnvironmentType resultTileType;
    public int effect_ID;
    public float damage;
    public DamageType damageType;

    public TileReactionResult(bool isReacted, EnvironmentType sourceTileType, EnvironmentType resultTileType, int effect_ID, float damage, DamageType damageType)
    {
        this.isReacted = isReacted;
        this.sourceTileType = sourceTileType;
        this.resultTileType = resultTileType;
        this.effect_ID = effect_ID;
        this.damage = damage;
        this.damageType = damageType;
    }

    // 실패
    public static TileReactionResult NoReaction => new TileReactionResult(false, EnvironmentType.none, EnvironmentType.none, -1, 0f, DamageType.None);
}
public static class TileRuleProccessor
{
    private static Dictionary<(DamageType, EnvironmentType), EnvRuleData> ruleDic => SODataManager.Instance.envRuleDataBase.ruleDic;

    public static void ProcessTileReactions(DamageInfo damageInfo, List<Tile> targetTiles)
    {
        // 중복 처리 방지용
        Dictionary<EnvironmentType, HashSet<Tile>> airClusters = new();
        Dictionary<EnvironmentType, HashSet<Tile>> groundClusters = new();

        foreach (var tile in targetTiles)
        {
            if (tile.airEffect != null)
                CollectCluster(tile, tile.airEffect.EnvType, airClusters, isAir: true);
            if (tile.groundEffect != null)
                CollectCluster(tile, tile.groundEffect.EnvType, groundClusters, isAir: false);
        }

        // Air 일괄 처리
        ProcessClusters(damageInfo, airClusters, isAir: true);

        // Ground 일괄 처리
        ProcessClusters(damageInfo, groundClusters, isAir: false);
    }
    public static void ProcessTileReactions(DamageInfo damageInfo, Tile targetTile)
    {
        // 중복 처리 방지용
        Dictionary<EnvironmentType, HashSet<Tile>> airClusters = new();
        Dictionary<EnvironmentType, HashSet<Tile>> groundClusters = new();


        if (targetTile.airEffect != null)
            CollectCluster(targetTile, targetTile.airEffect.EnvType, airClusters, isAir: true);
        if (targetTile.groundEffect != null)
            CollectCluster(targetTile, targetTile.groundEffect.EnvType, groundClusters, isAir: false);


        // Air 일괄 처리
        ProcessClusters(damageInfo, airClusters, isAir: true);

        // Ground 일괄 처리
        ProcessClusters(damageInfo, groundClusters, isAir: false);
    }

    private static void CollectCluster(Tile startTile, EnvironmentType envType, Dictionary<EnvironmentType, HashSet<Tile>> clusters, bool isAir)
    {
        if (envType == EnvironmentType.none)
            return;

        if (!clusters.ContainsKey(envType))
            clusters[envType] = new HashSet<Tile>();

        var connected = CollectConnectedTiles(startTile, envType, isAir);

        foreach (var tile in connected)
            clusters[envType].Add(tile);  // 일단 모으기만 함
    }


    private static HashSet<Tile> CollectConnectedTiles(Tile startTile, EnvironmentType envType, bool isAir)
    {
        Queue<Tile> queue = new();
        HashSet<Tile> visited = new();

        queue.Enqueue(startTile);
        visited.Add(startTile);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var neighbor in TileUtility.GetAdjacentTileList(current.curLevel, current))
            {
                if (visited.Contains(neighbor))
                    continue;
                var neighborEffect = isAir ? neighbor.airEffect : neighbor.groundEffect;
                if (neighborEffect == null) continue;
                var neighborEnvType = neighborEffect.EnvType;
                if (neighborEnvType == envType)
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return visited;
    }
    private static void ProcessClusters(DamageInfo damageInfo, Dictionary<EnvironmentType, HashSet<Tile>> clusters, bool isAir)
    {
        foreach (var cluster in clusters.Values)
        {
            foreach (var tile in cluster)
            {
                var result = CheckRule(damageInfo, tile, isAir);

                if (result.isReacted)
                {
                    tile.AffectOnTile(result, isAir);

                }
            }
        }
    }




    public static TileReactionResult CheckRule(DamageInfo damageInfo, Tile tile, bool isAir)
    {
        var env = isAir ? tile.airEffect : tile.groundEffect;
        if (!ruleDic.ContainsKey((damageInfo.damageType, env.EnvType)))
            return TileReactionResult.NoReaction;

        EnvRuleData rule = ruleDic[(damageInfo.damageType, env.EnvType)];
        bool isReacted = true;
        EnvironmentType resultTileType = rule.resultTileType;
        int effect_ID = rule.effect_ID;
        float damage = rule.damageMultiplier * damageInfo.value;
        DamageType damageType = damageInfo.damageType;

        TileReactionResult result = new TileReactionResult(isReacted, env.EnvType, resultTileType, effect_ID, damage, damageType);
        return result;
    }
}
