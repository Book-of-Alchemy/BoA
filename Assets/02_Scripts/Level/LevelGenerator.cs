using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Leaf root;
    public int roomCnt;
    public BiomeSet biomeSet;
    List<Leaf> seletedLeaf;
    Leaf startLeaf;
    Leaf endLeaf;
    List<(Leaf, Leaf)> Edges;

    public Level GenerateLevel(int rootWidth, int rootHeight, int minSize, int maxSize)
    {
        Level level = new Level();


        SetRoot(rootWidth, rootHeight);
        seletedLeaf = SeletedLeaf(roomCnt, SplitMap(minSize, maxSize));
        SelectStartAndEndLeaf(seletedLeaf, out startLeaf, out endLeaf);
        Edges = GenerateKruskalMST(seletedLeaf);

        return null;
    }

    public void SetRoot(int width, int height)
    {
        root = new Leaf(new RectInt(0, height, width, height));
    }

    public List<Leaf> SplitMap(int minSize, int maxSize)
    {
        List<Leaf> leaves = new List<Leaf>();
        Queue<Leaf> queue = new Queue<Leaf>();

        queue.Enqueue(root);
        leaves.Add(root);

        while (queue.Count > 0)
        {
            Leaf leaf = queue.Dequeue();

            if (leaf.rect.width > maxSize || leaf.rect.height > maxSize)
            {
                if (leaf.Split(minSize))
                {
                    queue.Enqueue(leaf.left);
                    queue.Enqueue(leaf.right);
                    leaves.Add(leaf.left);
                    leaves.Add(leaf.right);
                }
            }
        }


        return leaves;
    }

    int Compare(Leaf a, Leaf b)
    {
        return a.rect.x.CompareTo(b.rect.x);
    }

    List<Leaf> SeletedLeaf(int roomCnt, List<Leaf> leaves)
    {
        List<Leaf> selected = new List<Leaf>();
        HashSet<int> usedIndices = new HashSet<int>();

        while (selected.Count < roomCnt && usedIndices.Count < leaves.Count)
        {
            int index = Random.Range(0, leaves.Count);

            if (!usedIndices.Contains(index))
            {
                usedIndices.Add(index);
                selected.Add(leaves[index]);
            }
        }

        selected.Sort();
        return selected;
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

    public List<(Leaf, Leaf)> GenerateKruskalMST(List<Leaf> leaves,float extraConnectionChance = 0.3f)//최소신장트리 즉 모든 Leaf를 노드로 잡고 각 방의 거리를 오름차순으로 정렬하여 빠짐없이 연결하는 로직
    {
        List<(Leaf, Leaf, float)> edges = new();//튜플리스트 처음써봄 ㄹㅇ

        for (int i = 0; i < leaves.Count; i++)
        {
            for (int j = i + 1; j < leaves.Count; j++)
            {
                float dist = Vector2.Distance(leaves[i].rect.center, leaves[j].rect.center);
                edges.Add((leaves[i], leaves[j], dist));
            }
        }

        edges.Sort((a, b) => a.Item3.CompareTo(b.Item3));//거리에 따라 오름차순 정렬 튜플 람다식알아보다 머리터짐 ㄹㅇ 가까운방부터 순서대로 정렬한다.


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

        bool IsAdjacent(Leaf a, Leaf b)
        {
            return a.rect.Overlaps(ExpandRect(b.rect,1)); //b 크기를 1 늘려서 겹치는지 확인
        }

        RectInt ExpandRect(RectInt rect, int amount)
        {
            return new RectInt(rect.x - amount, rect.y - amount, rect.width + 2 * amount, rect.height + 2 * rect.height);
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
            if (IsAdjacent(a, b) && Random.value < extraConnectionChance)
            {
                mstEdges.Add((a, b)); //30퍼 확률로 연결된 인접한 leaf연결
            }
        }

        return mstEdges;
    }



    Dictionary<Vector2Int, Tile> SetTiles(List<Leaf> leaves, RoomPreset room = null)
    {
        Dictionary<Vector2Int, Tile> tiles = new ();


        return tiles;
    }
}
