using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreSmite : EnemySkill, ICooltime
{
    List<Tile> neighbors => attackBaseBehaviour.adjacentiveTile;
    List<Tile> targetTiles = new List<Tile>();
    bool isPreparing = false;
    public int lefttime => Mathf.Max(0, availableTime - TurnManager.Instance.globalTime); // 남은 턴
    public int coolTime { get; set; } // 쿨타임 시간
    public int availableTime { get; set; }

    protected override void Awake()
    {
        base.Awake();
        coolTime = 80;
        availableTime = 0;
    }
    public override SkillUseState CanUse()//캔유즈 실행후 enemy 애니메이션 실행 예정
    {
        int currentTime = TurnManager.Instance.globalTime;

        // 발동
        if (isPreparing)
        {
            availableTime = currentTime + coolTime;
            isPreparing = false;
            ItemManager.Instance.DestroyBossRange(gameObject.name);
            return SkillUseState.ReadyToCast;
        }

        // 준비 시작
        if (!isPreparing && currentTime >= availableTime)
        {
            foreach (Tile tile in neighbors)
            {
                if (tile.CharacterStatsOnTile is PlayerStats player && !player.IsHidden)
                {
                    isPreparing = true;
                    targetTiles = new List<Tile>();
                    foreach (Tile target in neighbors)
                    {
                        if (target.tileType == TileType.ground)
                            targetTiles.Add(target);
                    }
                    ItemManager.Instance.CreateBossRange(targetTiles, gameObject.name);// 시각 경고
                    return SkillUseState.Preparing;
                }
            }
        }

        // 사용할 수 없음
        return SkillUseState.CannotUse;
    }
    public override void Use() //스킬 애니메이션 중간에 실행 예정
    {
        DealDamage();
    }

    /// <summary>
    /// 실질적인 대미지를 주는 메서드 
    /// 원거리 공격시 use에서 이펙트프로젝타일매니저 launch후 콜백액션으로 달아주며 
    /// 근거리공격시 use내에서 dealdamage처리하도록
    /// </summary>
    public override void DealDamage()
    {
        
        foreach(Tile tile in targetTiles)
        {
            EffectProjectileManager.Instance.PlayEffect(tile.gridPosition, 30012);
            if (tile.CharacterStatsOnTile != null)
            {
                DamageInfo damageInfo = new DamageInfo(stats.AttackDamage,DamageType.None,stats, tile.CharacterStatsOnTile);
                tile.CharacterStatsOnTile.TakeDamage(damageInfo);
            }
        }
        attackBaseBehaviour.EndTurn();
    }
}
