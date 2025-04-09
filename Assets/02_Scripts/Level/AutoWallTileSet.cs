using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/AutoWallTileSet")]
public class AutoWallTileSet : ScriptableObject
{
    [Tooltip("연결 상태(0~15)에 따른 벽 스프라이트")]
    public List<Sprite> wallSprites = new List<Sprite>(16);

    public Sprite GetSprite(int bitmask)
    {
        return (bitmask >= 0 && bitmask < wallSprites.Count) ? wallSprites[bitmask] : null;
    }
}