using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/AutoWallTileSet")]
public class AutoWallTileSet : ScriptableObject
{
    public int id;
    public string name_kr;
    /// <summary>
    /// bitmask 순서는 상 우 하 좌
    /// 상 +1 우 +2 하 +4 좌 +8
    /// </summary>
    [Header("Front Wall Sprites")]
    public Sprite[] frontBaseSprites = new Sprite[16];
    public Sprite[] frontUpperSprites = new Sprite[16];

    [Header("Back Wall Sprites")]
    public Sprite[] backBaseSprites = new Sprite[16];
    public Sprite[] backUpperSprites = new Sprite[16];

    public Sprite GetBaseSprite(int bitmask, bool isFront)
    {
        var list = isFront ? frontBaseSprites : backBaseSprites;
        return (bitmask >= 0 && bitmask < list.Length) ? list[bitmask] : null;
    }

    public Sprite GetUpperSprite(int bitmask, bool isFront)
    {
        var list = isFront ? frontUpperSprites : backUpperSprites;
        return (bitmask >= 0 && bitmask < list.Length) ? list[bitmask] : null;
    }
}