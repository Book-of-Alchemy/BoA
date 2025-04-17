using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LeafSizeType
{
    small = 0,
    medium,
    large,
}
public class Leaf
{
    public RectInt rect;
    public Leaf left, right; // 분할된 자식
    public RoomPreset room;
    public Tile centerTile;
    public List<Tile> doorPoint = new List<Tile>();
    public RoomType roomType = RoomType.normal;
    public LeafSizeType leafSizeType => GetLeafSizeType();
    public int minSmallSize = 7;
    public int maxSmallSize = 9;
    public int minMediumSize = 10;
    public int maxMediumSize = 14;
    public int minLargeSize = 15;
    public int maxLargesize = 19;
    float smallSizeChance = 0.4f;
    float middleSizeChance = 0.45f;

    public Leaf(RectInt rect)
    {
        this.rect = rect;
    }

    public bool Split(int minSize = 6)
    {
        if (left != null || right != null) return false;

        int targetSize = GetTargetRoomSize();//확률적 분할 코드 추가

        bool splitHorizontally = UnityEngine.Random.value > 0.5f;
        if (rect.width > rect.height && rect.width / rect.height >= 1.25f)
            splitHorizontally = false;
        else if (rect.height > rect.width && rect.height / rect.width >= 1.25f)
            splitHorizontally = true;

        int max = (splitHorizontally ? rect.height : rect.width) - minSize;

        if (max <= minSize) return false;

        targetSize = Mathf.Clamp(GetTargetRoomSize(), minSize, max);
        int split = UnityEngine.Random.Range(targetSize, max);

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

    int GetTargetRoomSize()
    {
        float rand = UnityEngine.Random.value;

        if (rand < smallSizeChance) // 소형
            return UnityEngine.Random.Range(minSmallSize, maxSmallSize);
        else if (rand < smallSizeChance + middleSizeChance) // 중형
            return UnityEngine.Random.Range(minMediumSize, maxMediumSize);
        else // 대형
            return UnityEngine.Random.Range(minLargeSize, maxLargesize);
    }

    LeafSizeType GetLeafSizeType()
    {
        int size = rect.width < rect.height ? rect.width : rect.height;

        if(size <= maxSmallSize)
            return LeafSizeType.small;

        if(size <= maxMediumSize)
            return LeafSizeType.medium;

        if(size <= maxLargesize)
            return LeafSizeType.large;

        return LeafSizeType.large;
    }

}