using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{

    public static List<Tile> FindPath(Tile startTile, Tile goalTile, Level level, bool forPathfinding = true)//최대 탐색 거리 추가예정
    {
        if (startTile == null || goalTile == null || level == null) return null;

        var openList = new Dictionary<Vector2Int, AstarNode>();//리스트 중복시 성능 저하로 오픈리스트 딕셔너리처리 내부 필드를 통한 검색필요 변수명만 list
        var closedList = new HashSet<Vector2Int>();//방문 여부만 체크하므로 해쉬셋 gridPosition

        AstarNode startNode = new AstarNode(startTile)
        {
            gCost = 0,
            hCost = Heuristic(startTile, goalTile)
        };

        openList[startTile.gridPosition] = startNode;

        while (openList.Count > 0)
        {
            AstarNode current = GetLowestFCostNode(openList);//모든 오픈리스트를 돌며 fcost가 낮은 오픈리스트 반환

            if (current.tile == goalTile) return ReconstructPath(current);

            openList.Remove(current.tile.gridPosition);
            closedList.Add(current.tile.gridPosition);

            foreach (Tile neighbor in GetNeighbors(current.tile, level, forPathfinding))//길찾기용은 8dir 통로생성용은 4dir neighbor반환
            {
                if (closedList.Contains(neighbor.gridPosition)) continue;

                int tempGCost = current.gCost + neighbor.AstarCost;

                if (!openList.TryGetValue(neighbor.gridPosition, out AstarNode neighborNode))
                {
                    neighborNode = new AstarNode(neighbor)
                    {
                        gCost = tempGCost,
                        hCost = Heuristic(neighbor, goalTile),
                        parent = current
                    };

                    if (neighborNode.IsWalkable(forPathfinding) || neighborNode.tile == goalTile)
                    {
                        openList[neighbor.gridPosition] = neighborNode;
                    }
                }
                else if (tempGCost < neighborNode.gCost)
                {
                    neighborNode.gCost = tempGCost;
                    neighborNode.parent = current;
                }
            }
        }

        return null; //오픈리스트를 다돌았음에도 경로 없음 추가로 gCost가 일정량 이상 넘어가면 null처리 후 별도의 길찾기 로직실행
    }

    public static List<Tile> FindPathForCreature(Tile startTile, Tile goalTile, Level level, bool isDetectTrap = true)//최대 탐색 거리 추가예정
    {
        if (startTile == null || goalTile == null || level == null) return null;

        var openList = new Dictionary<Vector2Int, AstarNode>();//리스트 중복시 성능 저하로 오픈리스트 딕셔너리처리 내부 필드를 통한 검색필요 변수명만 list
        var closedList = new HashSet<Vector2Int>();//방문 여부만 체크하므로 해쉬셋 gridPosition

        AstarNode startNode = new AstarNode(startTile)
        {
            gCost = 0,
            hCost = Heuristic(startTile, goalTile)
        };

        openList[startTile.gridPosition] = startNode;

        while (openList.Count > 0)
        {
            AstarNode current = GetLowestFCostNode(openList);//모든 오픈리스트를 돌며 fcost가 낮은 오픈리스트 반환

            if (current.tile == goalTile) return ReconstructPath(current);

            openList.Remove(current.tile.gridPosition);
            closedList.Add(current.tile.gridPosition);

            foreach (Tile neighbor in GetNeighbors(current.tile, level, true))//길찾기용은 8dir 통로생성용은 4dir neighbor반환
            {
                if (closedList.Contains(neighbor.gridPosition)) continue;

                int tempGCost = current.gCost + neighbor.AstarCost;
                if (isDetectTrap && neighbor.TrpaOnTile != null)//함정 인지 + 함정 존재시 + 6
                    tempGCost += 6;

                if (!openList.TryGetValue(neighbor.gridPosition, out AstarNode neighborNode))
                {
                    neighborNode = new AstarNode(neighbor)
                    {
                        gCost = tempGCost,
                        hCost = Heuristic(neighbor, goalTile),
                        parent = current
                    };

                    if (neighborNode.IsWalkable(true) || neighborNode.tile == goalTile)
                    {
                        openList[neighbor.gridPosition] = neighborNode;
                    }
                }
                else if (tempGCost < neighborNode.gCost)
                {
                    neighborNode.gCost = tempGCost;
                    neighborNode.parent = current;
                }
            }
        }

        return null; //오픈리스트를 다돌았음에도 경로 없음 추가로 gCost가 일정량 이상 넘어가면 null처리 후 별도의 길찾기 로직실행
    }


    private static int Heuristic(Tile a, Tile b)//맨허튼 거리 필요할경우 수정
    {
        return Mathf.Abs(a.gridPosition.x - b.gridPosition.x) + Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
    }

    private static AstarNode GetLowestFCostNode(Dictionary<Vector2Int, AstarNode> dict)
    {
        AstarNode best = null;
        foreach (var node in dict.Values)
        {
            if (best == null || node.fCost < best.fCost || (node.fCost == best.fCost && node.hCost < best.hCost))
                best = node;
        }
        return best;
    }

    private static List<Tile> ReconstructPath(AstarNode node)
    {
        List<Tile> path = new List<Tile>();
        while (node != null)//목적지의 노드부터 부모 노드들을 추적하며 리스트에 추가
        {
            path.Add(node.tile);
            node = node.parent;
        }
        path.Reverse();//길찾기용 반전
        return path;
    }

    private static List<Tile> GetNeighbors(Tile tile, Level level, bool allowDiagonal)
    {
        List<Tile> neighbors = new List<Tile>();

        Vector2Int[] directions = allowDiagonal
            ? new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                new Vector2Int(1, 1), new Vector2Int(-1, 1),
                new Vector2Int(1, -1), new Vector2Int(-1, -1)
            }
            : new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
            };

        foreach (var dir in directions)
        {
            Vector2Int pos = tile.gridPosition + dir;
            if (level.tiles.TryGetValue(pos, out Tile neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}
