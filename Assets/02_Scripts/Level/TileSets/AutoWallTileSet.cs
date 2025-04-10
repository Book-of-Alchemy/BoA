using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/AutoWallTileSet")]
public class AutoWallTileSet : ScriptableObject
{
    public int id;
    public string name_kr;

    /// <summary>
    /// 연결 상태(0~15)에 따른 벽 스프라이트
    /// </summary>
    public List<Sprite> wallSprites = new List<Sprite>(16);


    public Sprite GetSprite(int bitmask)
    {
        return (bitmask >= 0 && bitmask < wallSprites.Count) ? wallSprites[bitmask] : null;
    }
}