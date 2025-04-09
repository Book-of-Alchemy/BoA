using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf
{
    public RectInt rect; 
    public Leaf left, right; // 분할된 자식
    public RectInt? room; 

    public Leaf(RectInt rect)
    {
        this.rect = rect;
    }

    public bool Split(int minSize)
    {
        if (left != null || right != null)
            return false;
        //확률적 분할 코드 추가?

        bool splitHorizontally = UnityEngine.Random.value > 0.5f;
        if (rect.width > rect.height && rect.width / rect.height >= 1.25f)
            splitHorizontally = false;
        else if (rect.height > rect.width && rect.height / rect.width >= 1.25f)
            splitHorizontally = true;

        int max = (splitHorizontally ? rect.height : rect.width) - minSize;
        if (max <= minSize)
            return false; 


        int split = UnityEngine.Random.Range(minSize, max);

        if (splitHorizontally)
        {
            left = new Leaf(new RectInt(rect.x, rect.y, rect.width, split));
            right = new Leaf(new RectInt(rect.x, rect.y + split, rect.width, rect.height - split));
        }
        else
        {
            left = new Leaf(new RectInt(rect.x, rect.y, split, rect.height));
            right = new Leaf(new RectInt(rect.x + split, rect.y, rect.width - split, rect.height));
        }

        return true;
    }
}