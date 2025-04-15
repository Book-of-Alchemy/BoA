using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AstarNode
{
    public Tile tile;
    public AstarNode parent;

    public int gCost;         // 시작부터 현재까지의 누적 비용
    public int hCost;         // 휴리스틱 (목표까지 예상 거리)
    public int fCost => gCost + hCost;

    public int astarCost => tile != null ? tile.AstarCost : int.MaxValue;      // 현재 타일의 이동 비용 (Tile의 AstarCost를 복사)

    public AstarNode(Tile tile)
    {
        this.tile = tile;
    }

    /// <summary>
    /// 길찾을땐 true 복도 생성용 false
    /// </summary>
    /// <param name="forPathfinding"></param>
    /// <returns></returns>
    public bool IsWalkable(bool forPathfinding = true)//통로생성용으로 사용시 empty는 이동 가능 길찾기용은 불가능 isDoorPoint일경우 wall도 이동가능으로 취급
    {

        if (tile == null) return false;

        return forPathfinding ? tile.IsWalkable : tile.isDoorPoint || tile.tileType == TileType.empty || tile.IsWalkable;
    }
}
