using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreSmite : EnemySkill, ICooltime
{
    List<Tile> neighbors => attackBaseBehaviour.adjacentiveTile;
    bool isReady = false;
    public int lefttime => Mathf.Max(0, availableTime - TurnManager.Instance.globalTime); // 남은 턴
    public int coolTime { get; set; } // 쿨타임 시간
    public int availableTime { get; set; }

    protected override void Awake()
    {
        base.Awake();
        coolTime = 80;
        availableTime = 0;
    }
    public override bool CanUse()//캔유즈 실행후 enemy 애니메이션 실행 예정
    {
        if (isReady)
        {
            availableTime = TurnManager.Instance.globalTime + coolTime;
            return true;
        }


        foreach (Tile tile in neighbors)
        {
            if (tile.CharacterStatsOnTile is PlayerStats player)
            {
                if (player.IsHidden)
                    continue;

                //여기서 준비를 보여줘야함
                isReady = true;
            }
        }

        return false;
    }
    public override void Use() //스킬 애니메이션 중간에 실행 예정
    {

    }

    /// <summary>
    /// 실질적인 대미지를 주는 메서드 
    /// 원거리 공격시 use에서 이펙트프로젝타일매니저 launch후 콜백액션으로 달아주며 
    /// 근거리공격시 use내에서 dealdamage처리하도록
    /// </summary>
    public override void DealDamage()
    {

    }
}
