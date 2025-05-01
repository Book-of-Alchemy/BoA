using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    public Leaf root;
    public int roomCnt;
    public BiomeSet curBiomeSet;
    //향후 퀘스트 db를 받아서 퀘스트 관련하여 레벨 세팅

    private void Start()
    {
        TileManger.Instance.levelGenerator = this;
    }

    public void SetLevelGenerator(BiomeSet biomeSet, int roomCnt, int rootWidth, int rootHeight)
    {
        SetRoot(rootWidth, rootHeight);
        this.roomCnt = roomCnt;
        curBiomeSet = biomeSet;
    }

    public Level GenerateLevel(int minSize = 7, QuestData questData =null)
    {
        Level level = new GameObject("Level").AddComponent<Level>();
        level.questData = SODataManager.Instance.questDataBase.questData[0];//임시코드
        level.biomeSet = curBiomeSet;
        level.tileDataBase = TileManger.Instance.tileData;

        List<Leaf> seletedLeaves;
        List<Leaf> extraLeaves;
        Leaf startLeaf;
        Leaf endLeaf;
        Leaf trapLeaf;
        Leaf treasureLeaf;
        Leaf scretLeaf;
        List<(Leaf, Leaf)> Edges;


        SeletedLeaf(roomCnt, SplitMap(minSize), out seletedLeaves, out extraLeaves);
        SelectStartAndEndLeaf(seletedLeaves, out startLeaf, out endLeaf);
        Edges = GenerateKruskalMST(seletedLeaves);
        SelectTrapAndTreasureLeaf(extraLeaves, out trapLeaf, out treasureLeaf);
        SelectScretLeaf(extraLeaves, out scretLeaf);

        if (trapLeaf != null && treasureLeaf != null)
        {
            Edges.Add((trapLeaf, treasureLeaf));
            Edges.Add((trapLeaf, FindClosestLeaf(trapLeaf, seletedLeaves)));
        }

        if (scretLeaf != null)
            Edges.Add((scretLeaf, FindClosestLeaf(scretLeaf, seletedLeaves)));

        if (trapLeaf != null)
            seletedLeaves.Add(trapLeaf);

        if (treasureLeaf != null)
            seletedLeaves.Add(treasureLeaf);

        if (scretLeaf != null)
            seletedLeaves.Add(scretLeaf);

        
        SetRoomOnLeaves(seletedLeaves);

        Dictionary<Vector2Int, Tile> tiles = GenerateTilesFromLeaf(root);//우선 empty 깔고 시작

        SetTiles(tiles, seletedLeaves);

        level.tiles = tiles;

        ConnectEdgesWithPaths(Edges, level);
        FillWallByCorridorOnLevel(level);

        level.startTile = startLeaf.centerTile != null ? startLeaf.centerTile : null;
        level.endTile = endLeaf.centerTile != null ? endLeaf.centerTile : null;

        foreach (var leaf in seletedLeaves)
        {
            foreach (var tile in leaf.trapPoint)
            {
                level.trapPoint.Add(tile);
            }
        }

        SetLevelOnTiles(tiles, level);

        level.seletedLeaves = seletedLeaves;
        level.startLeaf = startLeaf;

        return level;
    }

    public void SetRoot(int width, int height)
    {
        root = new Leaf(new RectInt(0, 0, width, height));
    }

    public List<Leaf> SplitMap(int minSize)
    {
        List<Leaf> leaves = new List<Leaf>();
        Queue<Leaf> queue = new Queue<Leaf>();

        queue.Enqueue(root);
        leaves.Add(root);

        while (queue.Count > 0)
        {
            Leaf leaf = queue.Dequeue();

            if (leaf.Split(minSize))
            {
                queue.Enqueue(leaf.left);
                queue.Enqueue(leaf.right);
                leaves.Add(leaf.left);
                leaves.Add(leaf.right);
                leaves.Remove(leaf);
            }

        }

        return leaves;
    }

    void SeletedLeaf(int roomCnt, List<Leaf> leaves, out List<Leaf> selected, out List<Leaf> extra)
    {
        selected = new List<Leaf>();
        extra = new List<Leaf>(leaves);
        HashSet<int> usedIndices = new HashSet<int>();

        while (selected.Count < roomCnt && usedIndices.Count < leaves.Count)
        {
            int index = UnityEngine.Random.Range(0, leaves.Count);

            if (!usedIndices.Contains(index))
            {
                usedIndices.Add(index);
                selected.Add(leaves[index]);
            }
        }

        foreach (Leaf leaf in selected)
        {
            extra.Remove(leaf);
        }
    }

    void SelectStartAndEndLeaf(List<Leaf> selectedLeaves, out Leaf startLeaf, out Leaf endLeaf)
    {
        float maxDistance = 0f;
        startLeaf = null;
        endLeaf = null;

        for (int i = 0; i < selectedLeaves.Count; i++)
        {
            for (int j = i + 1; j < selectedLeaves.Count; j++)
            {
                Vector2 centerA = selectedLeaves[i].rect.center;
                Vector2 centerB = selectedLeaves[j].rect.center;

                float distance = Vector2.Distance(centerA, centerB);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    startLeaf = selectedLeaves[i];
                    endLeaf = selectedLeaves[j];
                }
            }
        }
    }

    void SelectTrapAndTreasureLeaf(List<Leaf> extraLeaves, out Leaf trapLeaf, out Leaf treasureLeaf)
    {
        trapLeaf = null;
        treasureLeaf = null;

        if (extraLeaves == null || extraLeaves.Count < 2) return;


        int maxTries = 20;
        int tryCount = 0;

        while (tryCount < maxTries)
        {
            tryCount++;

            Leaf baseLeaf = extraLeaves[UnityEngine.Random.Range(0, extraLeaves.Count)];

            foreach (var leaf in extraLeaves)
            {
                if (leaf == baseLeaf) continue;

                if (IsAdjacent(baseLeaf, leaf))
                {
                    trapLeaf = baseLeaf;
                    treasureLeaf = leaf;
                    return;
                }
            }
        }

        if (trapLeaf == null || treasureLeaf == null)
            return;

        trapLeaf.roomType = RoomType.trap;
        treasureLeaf.roomType = RoomType.treasure;
        extraLeaves.Remove(trapLeaf);
        extraLeaves.Remove(treasureLeaf);
    }

    void SelectScretLeaf(List<Leaf> extraLeaves, out Leaf scretLeaf, float scretLeafChance = 0.5f)
    {
        scretLeaf = null;

        if (extraLeaves == null || extraLeaves.Count <= 0) return;

        if (UnityEngine.Random.value < scretLeafChance)
            scretLeaf = extraLeaves[UnityEngine.Random.Range(0, extraLeaves.Count)];
        else
            return;
        scretLeaf.roomType = RoomType.secret;
        extraLeaves.Remove(scretLeaf);
        Debug.Log($"scretLeaf = {scretLeaf.rect.center}");
    }

    Leaf FindClosestLeaf(Leaf leaf, List<Leaf> leaves)
    {
        float minDistance = float.MaxValue;
        Leaf closestLeaf = null;
        foreach (var compare in leaves)
        {
            if (compare == leaf) continue;

            float distance = Vector2.Distance(leaf.rect.center, compare.rect.center);
            if (distance < minDistance)
                closestLeaf = compare;
        }

        return closestLeaf;
    }


    public List<(Leaf, Leaf)> GenerateKruskalMST(List<Leaf> leaves, float extraConnectionChance = 0.3f)//최소신장트리 즉 모든 Leaf를 노드로 잡고 각 방의 거리를 오름차순으로 정렬하여 빠짐없이 연결하는 로직
    {
        List<(Leaf, Leaf, float)> edges = new();

        for (int i = 0; i < leaves.Count; i++)
        {
            for (int j = i + 1; j < leaves.Count; j++)
            {
                float dist = Vector2.Distance(leaves[i].rect.center, leaves[j].rect.center);
                edges.Add((leaves[i], leaves[j], dist));
            }
        }

        edges.Sort((a, b) => a.Item3.CompareTo(b.Item3));//거리에 따라 오름차순 정렬  가까운방부터 순서대로 정렬한다.


        Dictionary<Leaf, Leaf> parent = new(); //  유니온-파인드 구조 초기화 쿠르스칼에서 쓰는 방식 Prisma에선 start에서 가장가까운것을 연결하나 연결구조가 좋지않다.
        foreach (var leaf in leaves)//딕셔너리에 본인 Leaf를 키값으로 하며 value 값으로 지정 예를 들어 각 방이 있으면 자신이 대장인셈
        {
            parent[leaf] = leaf;//key값은 현재 졸개 value 값은 대장을 의미
        }


        Leaf Find(Leaf x)// 유니온-파인드 
        {
            if (parent[x] != x)//x가 현재 자신의 대장이이라면 x를 반환
                parent[x] = Find(parent[x]); //x의 대장을 x의 대장의 대장으로 한다. 재귀하여 최종 대장을 찾는다.
            return parent[x];//x의 최종 방장을 반환 두 그룹을 연합시킨다.
        }

        void Union(Leaf a, Leaf b)
        {
            Leaf rootA = Find(a);//각 멤버의 최종 대장을 찾음 
            Leaf rootB = Find(b);
            if (rootA != rootB)//최종 대장이 다르면 한쪽 대장을 상대 대장으로 지정
                parent[rootA] = rootB;
        }

        List<(Leaf, Leaf)> mstEdges = new();//간선 리스트
        List<(Leaf, Leaf)> extraEdges = new();//추가 연결 위한 리스트
        foreach (var (a, b, _) in edges)//_는 사용하지 않는다는 의미 아무거나 넣어도 무방 튜플 역시 var()로 통일가능
        {
            if (Find(a) != Find(b))//a와 b의 총대장이 다를경우
            {
                Union(a, b);//연합시킴
                mstEdges.Add((a, b));//거리가 가장 짧은 leaf 간선부터 연결시키며 모든 노드들이 연결될때까지 반복함
            }
            else
            {
                extraEdges.Add((a, b));
            }
        }

        foreach (var (a, b) in extraEdges)
        {
            if (IsAdjacent(a, b) && UnityEngine.Random.value < extraConnectionChance)
            {
                mstEdges.Add((a, b)); //30퍼 확률로 연결된 인접한 leaf연결
            }
        }

        return mstEdges;
    }
    bool IsAdjacent(Leaf a, Leaf b)
    {
        return a.rect.Overlaps(ExpandRect(b.rect, 1)); //b 크기를 1 늘려서 겹치는지 확인
    }

    RectInt ExpandRect(RectInt rect, int amount)
    {
        return new RectInt(rect.x - amount, rect.y - amount, rect.width + 2 * amount, rect.height + 2 * rect.height);
    }

    RoomPreset GetFittableRoom(Leaf leaf, RoomType type)
    {
        if (curBiomeSet == null) return null;

        List<RoomPreset> roomList = curBiomeSet.GetPresets(type);
        List<RoomPreset> fittableRoomList = new List<RoomPreset>();

        foreach (var list in roomList)
        {
            if (list.roomSize.x > leaf.rect.width || list.roomSize.y > leaf.rect.height)
                continue;

            fittableRoomList.Add(list);
        }

        if (fittableRoomList == null || fittableRoomList.Count <= 0) return null;

        RoomPreset room = null;

        int maxTries = 10;
        int tryCount = 0;

        while (tryCount < maxTries)
        {
            tryCount++;

            room = fittableRoomList[UnityEngine.Random.Range(0, fittableRoomList.Count)];
            Vector2Int roomSize = room.roomSize;

            if (roomSize.x < leaf.rect.width - 3 && roomSize.y < leaf.rect.height - 3)
                continue; // 너무 작음
            return room;
        }

        return room;
    }

    void SetRoomOnLeaves(List<Leaf> leaves)
    {
        foreach (Leaf leaf in leaves)
        {
            switch (leaf.roomType)
            {
                case RoomType.secret:
                    leaf.room = GetFittableRoom(leaf, RoomType.secret);
                    if (leaf.room == null)
                    {
                        leaf.room = GetFittableRoom(leaf, RoomType.normal);
                    }
                    break;
                case RoomType.trap:
                    leaf.room = GetFittableRoom(leaf, RoomType.trap);
                    if (leaf.room == null)
                    {
                        leaf.room = GetFittableRoom(leaf, RoomType.normal);
                    }
                    break;
                case RoomType.treasure:
                    leaf.room = GetFittableRoom(leaf, RoomType.treasure);
                    if (leaf.room == null)
                    {
                        leaf.room = GetFittableRoom(leaf, RoomType.normal);
                    }
                    break;
                default:
                    leaf.room = GetFittableRoom(leaf, RoomType.normal);
                    break;
            }
        }
    }

    Dictionary<Vector2Int, Tile> GenerateTilesFromLeaf(Leaf leaf)
    {
        Dictionary<Vector2Int, Tile> tiles = new();
        RectInt rect = leaf.rect;

        if (leaf.room != null && leaf.room.tileInfo != null)
        {
            int maxX = rect.width - leaf.room.roomSize.x;
            int maxY = rect.height - leaf.room.roomSize.y;
            Vector2Int randOffset = new Vector2Int(UnityEngine.Random.Range(0, maxX), UnityEngine.Random.Range(0, maxY));
            Vector2Int centerPos = rect.position + randOffset + (leaf.room.roomSize) / 2;
            // RoomPreset 기반 타일 생성
            foreach (var tileInfo in leaf.room.tileInfo)
            {
                Vector2Int pos = rect.position + randOffset + tileInfo.Key;
                TileInfoForRoom info = tileInfo.Value;

                Tile tile = new Tile
                {
                    gridPosition = pos,
                    tileType = info.tileType,
                    environmentType = info.environmentType,
                    isDoorPoint = info.isDoorPoint,
                    canSeeThrough = (info.tileType == TileType.wall ? false : true),
                    isOccupied = (info.tileType == TileType.wall ? true : false),
                    isExplored = false,
                    isOnSight = false
                };

                tiles[pos] = tile;

                if (tile.isDoorPoint)
                    leaf.doorPoint.Add(tile);

                if (pos == centerPos)
                    leaf.centerTile = tile;
            }

            PlaceTrapsInRoom(leaf, tiles);
        }
        else
        {
            for (int x = rect.x; x < rect.x + rect.width; x++)
            {
                for (int y = rect.y; y < rect.y + rect.height; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    Tile tile = new Tile
                    {
                        gridPosition = pos,
                        tileType = TileType.empty,
                        environmentType = EnvironmentType.none,
                        isDoorPoint = false,
                        isOccupied = false,
                        isExplored = false,
                        isOnSight = false
                    };

                    tiles[pos] = tile;
                }
            }
        }


        return tiles;
    }

    void PlaceTrapsInRoom(Leaf leaf, Dictionary<Vector2Int, Tile> tiles)
    {
        if (leaf.room == null || leaf.centerTile == null) return;

        if (leaf.roomType == RoomType.secret || leaf.roomType == RoomType.treasure || leaf.roomType == RoomType.trap) return;

        int spawnQuantity = (leaf.leafSizeType) switch
        {
            LeafSizeType.small => UnityEngine.Random.Range(0, 1),
            LeafSizeType.medium => UnityEngine.Random.Range(1, 2),
            LeafSizeType.large => UnityEngine.Random.Range(2, 3),
            _ => 0
        };

        List<Tile> candidates = new List<Tile>();

        foreach (var kvp in tiles)
        {
            if (!kvp.Value.isOccupied)
                candidates.Add(kvp.Value);
        }

        candidates.Remove(leaf.centerTile);
        float spawnRate = 0.7f;

        for (int i = 0; i < spawnQuantity; i++)
        {
            if (UnityEngine.Random.value > spawnRate)
                continue;

            if (candidates.Count == 0)
                break;

            Tile targetTile = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            candidates.Remove(targetTile);

            leaf.trapPoint.Add(targetTile);
        }
    }

    Dictionary<Vector2Int, Tile> GatherTiles(List<Leaf> leaves)
    {
        Dictionary<Vector2Int, Tile> allTiles = new();

        foreach (var leaf in leaves)
        {
            Dictionary<Vector2Int, Tile> leafTiles = GenerateTilesFromLeaf(leaf);

            foreach (var tiles in leafTiles)
            {
                allTiles[tiles.Key] = tiles.Value;
            }
        }

        return allTiles;
    }

    void SetTiles(Dictionary<Vector2Int, Tile> tiles, List<Leaf> leaves)
    {
        foreach (var tile in GatherTiles(leaves))
        {
            tiles[tile.Key] = tile.Value;
        }
    }

    void ConnectEdgesWithPaths(List<(Leaf, Leaf)> edges, Level level)
    {
        foreach (var (a, b) in edges)
        {
            if (a.centerTile == null || b.centerTile == null) continue;

            Tile doorA = FindClosestDoorPoint(a, b.centerTile.gridPosition);
            Tile doorB = FindClosestDoorPoint(b, a.centerTile.gridPosition);

            if (doorA == null || doorB == null) continue;

            List<Tile> path = AStarPathfinder.FindPath(doorA, doorB, level, false);

            if (path == null)
                continue;

            if (path.Count >= 2)
            {
                foreach (var tile in path)
                {
                    TurnToGroundTile(tile);
                    level.corridorTiles.Add(tile);
                }

                level.tiles[doorA.gridPosition].isDoorPoint = false;
                level.tiles[doorB.gridPosition].isDoorPoint = false;
            }
        }
    }

    void TurnToGroundTile(Tile tile)
    {
        if (tile == null) return;

        tile.tileType = TileType.ground;
        tile.canSeeThrough = true;
        tile.isOccupied = false;
    }

    Tile FindClosestDoorPoint(Leaf leaf, Vector2Int target)
    {
        float minDist = float.MaxValue;
        Tile closest = null;

        foreach (var door in leaf.doorPoint)
        {
            float dist = Vector2Int.Distance(door.gridPosition, target);
            if (dist < minDist)
            {
                minDist = dist;
                closest = door;
            }
        }

        if (closest != null)
        {
            leaf.doorPoint.Remove(closest);
        }

        return closest;
    }

    void FillWallByCorridorOnLevel(Level level)
    {
        // Dictionary의 Value만 리스트로 복사
        List<Tile> tileList = new List<Tile>(level.tiles.Values);

        foreach (var tile in tileList)
        {
            FillWallByCorridor(tile, level); 
        }
    }


    void FillWallByCorridor(Tile tile, Level level)
    {
        if (tile.tileType != TileType.ground) return;

        Dictionary<Vector2Int, Tile> tiles = level.tiles;
        List<Tile> checkList = TileUtility.GetAdjacentTileList(level, tile, true);
        List<Vector2Int> directions = GetEightDirection(); // 방향 목록

        for (int i = 0; i < checkList.Count; i++)
        {
            Tile check = checkList[i];
            Vector2Int offset = directions[i];
            Vector2Int newPos = tile.gridPosition + offset;

            if (check == null)
            {
                // 해당 위치에 Tile이 없으므로 새로 만들어서 추가
                Tile newWallTile = CreateWallTile(newPos);
                tiles[newPos] = newWallTile;
                continue;
            }

            if (check.tileType == TileType.empty)
            {
                check.tileType = TileType.wall;
                check.canSeeThrough = false;
            }
        }
    }

    List<Vector2Int> GetFourDirections()
    {
        return new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };
    }
    List<Vector2Int> GetEightDirection()
    {
        return new List<Vector2Int>
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1)
    };
    }


    Tile CreateWallTile(Vector2Int pos)
    {
        return new Tile
        {
            gridPosition = pos,
            tileType = TileType.wall,
            environmentType = EnvironmentType.none,
            isDoorPoint = false,
            isOccupied = false,
            canSeeThrough = false,
            isExplored = false,
            isOnSight = false
        };
    }

    void SetLevelOnTiles(Dictionary<Vector2Int, Tile> tiles, Level level)
    {
        foreach (var kvp in tiles)
        {
            kvp.Value.curLevel = level;
        }
    }

    /// <summary>
    /// 임시코드 새로운 로직의 절차적 맵생성
    /// </summary>
    public GameObject roomPrefab;
    public int maxBranches = 5;
    public int maxDepth = 5;
    public float roomSpacing = 5f;

    private HashSet<Vector2Int> visited = new HashSet<Vector2Int>();


    void GenerateMap(int maxRoomCount)
    {
        Queue<Leaf> roomQueue = new Queue<Leaf>();
        List<Leaf> generatedRooms = new List<Leaf>();
        int roomCount = 0;

        Leaf firstLeaf = new Leaf();
        firstLeaf.room = GetRandomRoom();
        firstLeaf.rect = new RectInt(0, 0, firstLeaf.room.roomSize.x, firstLeaf.room.roomSize.y);

        roomQueue.Enqueue(firstLeaf);
        generatedRooms.Add(firstLeaf);
        roomCount++;

        while (roomQueue.Count > 0 && roomCount < maxRoomCount)
        {
            Leaf current = roomQueue.Dequeue();
            List<Vector2Int> directions = GetFourDirections();
            Shuffle(directions);

            foreach (var dir in directions)
            {
                if (roomCount >= maxRoomCount)
                    break;

                RoomPreset nextRoom = GetRandomRoom();
                Vector2Int offset = GetOffset(current.room.roomSize, nextRoom.roomSize, dir);

                Vector2Int nextPos = new Vector2Int(
                    current.rect.position.x + offset.x,
                    current.rect.position.y + offset.y
                );

                RectInt newRect = new RectInt(nextPos.x, nextPos.y, nextRoom.roomSize.x, nextRoom.roomSize.y);

                // 방이 겹치지 않도록 검사
                bool overlaps = generatedRooms.Exists(r => r.rect.Overlaps(newRect));
                if (overlaps)
                    continue;

                Leaf newLeaf = new Leaf { room = nextRoom, rect = newRect };
                roomQueue.Enqueue(newLeaf);
                generatedRooms.Add(newLeaf);
                roomCount++;
            }
        }

        // 생성된 방 리스트 = generatedRooms
    }


    RoomPreset GetRandomRoom()
    {
        List<RoomPreset> roomList = curBiomeSet.roomsBySize[GetRandomSizeRoom(0.4f, 0.45f)];
        return roomList[UnityEngine.Random.Range(0, roomList.Count - 1)];
    }

    RoomSizeType GetRandomSizeRoom(float smallRoomChance, float mediumRoomChance)
    {
        float chance = UnityEngine.Random.value;

        if (chance < smallRoomChance)
            return RoomSizeType.small;
        else if (chance < mediumRoomChance + smallRoomChance)
            return RoomSizeType.medium;
        else
            return RoomSizeType.large;
    }

    void Shuffle(List<Vector2Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Vector2Int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    Vector2Int GetOffset(Vector2Int fromSize, Vector2Int toSize, Vector2Int direction)
    {
        // 방 사이에 2칸 간격 유지
        int gap = 2;

        if (direction == Vector2Int.up)
            return new Vector2Int(0, fromSize.y + gap);
        if (direction == Vector2Int.down)
            return new Vector2Int(0, -(toSize.y + gap));
        if (direction == Vector2Int.right)
            return new Vector2Int(fromSize.x + gap, 0);
        if (direction == Vector2Int.left)
            return new Vector2Int(-(toSize.x + gap), 0);

        return Vector2Int.zero;
    }
}
