using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public static List<Tile> FindPath(Tile startTile, Tile goalTile, Level level, bool forPathfinding = true)
    {
        if (startTile == null || goalTile == null || level == null) return null;

        var openList = new Dictionary<Vector2Int, AstarNode>();
        var closedList = new HashSet<Vector2Int>();

        AstarNode startNode = new AstarNode(startTile)
        {
            gCost = 0,
            hCost = Heuristic(startTile, goalTile)
        };

        openList[startTile.gridPosition] = startNode;

        while (openList.Count > 0)
        {
            AstarNode current = GetLowestFCostNode(openList);

            if (current.tile == goalTile)
                return ReconstructPath(current);

            openList.Remove(current.tile.gridPosition);
            closedList.Add(current.tile.gridPosition);

            foreach (Tile neighbor in GetNeighbors(current.tile, level, forPathfinding))
            {
                if (closedList.Contains(neighbor.gridPosition)) continue;

                int tempG = current.gCost + neighbor.AstarCost;

                if (!openList.TryGetValue(neighbor.gridPosition, out var neighborNode))
                {
                    neighborNode = new AstarNode(neighbor)
                    {
                        gCost = tempG,
                        hCost = Heuristic(neighbor, goalTile),
                        parent = current
                    };

                    if (neighborNode.IsWalkable(forPathfinding))
                    {
                        openList[neighbor.gridPosition] = neighborNode;
                    }
                }
                else if (tempG < neighborNode.gCost)
                {
                    neighborNode.gCost = tempG;
                    neighborNode.parent = current;
                }
            }
        }

        return null; // 경로 없음
    }

    private static int Heuristic(Tile a, Tile b)
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
