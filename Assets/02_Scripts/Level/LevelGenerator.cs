using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Leaf root;
    public int roomCnt;
    List<Leaf> seletedLeaf;
    Leaf startLeaf;
    Leaf endLeaf;


    public Level GenerateLevel(int rootWidth, int rootHeight, int minSize, int maxSize)
    {
        Level level = new Level();
        

        SetRoot(rootWidth, rootHeight);
        seletedLeaf = SeletedLeaf(roomCnt, SplitMap(minSize, maxSize));
        SelectStartAndEndLeaf(seletedLeaf,out startLeaf,out endLeaf);


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

    void SetRoomPreset()
    {
        seletedLeaf.Remove(startLeaf);
        seletedLeaf.Remove(endLeaf);

    }
}
