using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Leaf root;


    public Level GenerateLevel(int rootWidth, int rootHeight, int minSize, int m)
    {
        Level level = new Level();

        return null;
    }

    public void SetRoot(int width, int height)
    {
        root = new Leaf(new RectInt(0, height, width, height));
    }

    public List<Leaf> SplitMap( int minSize, int maxSize)
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

    List<Leaf> SeletedLeaf()
    {
        return null;
    }
}
