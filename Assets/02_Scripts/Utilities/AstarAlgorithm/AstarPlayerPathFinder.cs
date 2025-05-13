using System.Collections.Generic;
using UnityEngine;

public static class AstarPlayerPathFinder
{
    /// <summary>
    /// 플레이어 전용 경로 탐색
    /// - Reveal되지 않은 타일은 이동 불가
    /// - 보이는 함정은 이동 불가
    /// - 벽(TileType.Wall)은 이동 불가
    /// </summary>
    public static List<Tile> FindPath(
        Tile startTile,
        Tile goalTile,
        Level level,
        bool allowDiagonal = true)
    {
        if (startTile == null || goalTile == null || level == null)
            return null;

        var openList = new Dictionary<Vector2Int, AstarNode>();
        var closedList = new HashSet<Vector2Int>();

        var startNode = new AstarNode(startTile)
        {
            gCost = 0,
            hCost = Heuristic(startTile, goalTile)
        };
        openList[startTile.gridPosition] = startNode;

        while (openList.Count > 0)
        {
            // 최저 노드 추출
            var current = GetLowestFCostNode(openList);

            // 목표 도달
            if (current.tile == goalTile)
                return ReconstructPath(current);

            openList.Remove(current.tile.gridPosition);
            closedList.Add(current.tile.gridPosition);

            foreach (Tile neighbor in GetNeighbors(current.tile, level, allowDiagonal))
            {
                // 이미 처리된 타일
                if (closedList.Contains(neighbor.gridPosition))
                    continue;

                // 벽/미탐색/탐지된 함정 기존 필터
                if (neighbor.tileType == TileType.wall)
                    continue;
                if (!neighbor.IsExplored)
                    continue;
                if (neighbor.TrpaOnTile != null && neighbor.TrpaOnTile.IsDetected)
                    continue;

                //적(또는 다른 캐릭터)이 있는 타일 건너뛰기
                if (!neighbor.IsWalkable && neighbor != goalTile)
                    continue;

                // gCost 계산
                int tempGCost = current.gCost + neighbor.AstarCost;

                // openList 등록/업데이트 로직
                if (!openList.TryGetValue(neighbor.gridPosition, out AstarNode neighborNode))
                {
                    neighborNode = new AstarNode(neighbor)
                    {
                        gCost = tempGCost,
                        hCost = Heuristic(neighbor, goalTile),
                        parent = current
                    };
                    openList[neighbor.gridPosition] = neighborNode;
                }
                else if (tempGCost < neighborNode.gCost)
                {
                    neighborNode.gCost = tempGCost;
                    neighborNode.parent = current;
                }
            }

        }

        // 경로 없음
        return null;
    }

    private static int Heuristic(Tile a, Tile b)
    {
        // 맨해튼 거리
        return Mathf.Abs(a.gridPosition.x - b.gridPosition.x)
             + Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
    }

    private static AstarNode GetLowestFCostNode(Dictionary<Vector2Int, AstarNode> dict)
    {
        AstarNode best = null;
        foreach (var node in dict.Values)
        {
            if (best == null
             || node.fCost < best.fCost
             || (node.fCost == best.fCost && node.hCost < best.hCost))
                best = node;
        }
        return best;
    }

    private static List<Tile> ReconstructPath(AstarNode node)
    {
        var path = new List<Tile>();
        while (node != null)
        {
            path.Add(node.tile);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    private static List<Tile> GetNeighbors(Tile tile, Level level, bool allowDiagonal)
    {
        var neighbors = new List<Tile>();
        var dirs = allowDiagonal
            ? new[] {
                Vector2Int.up, Vector2Int.down,
                Vector2Int.left, Vector2Int.right,
                new Vector2Int( 1,  1), new Vector2Int(-1,  1),
                new Vector2Int( 1, -1), new Vector2Int(-1, -1)
              }
            : new[] {
                Vector2Int.up, Vector2Int.down,
                Vector2Int.left, Vector2Int.right
              };

        foreach (var d in dirs)
        {
            var pos = tile.gridPosition + d;
            if (level.tiles.TryGetValue(pos, out var nbr))
                neighbors.Add(nbr);
        }
        return neighbors;
    }
}
