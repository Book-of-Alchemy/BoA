using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/AutoEnvironmentalSet")]
public class AutoEnvironmentalSet : ScriptableObject
{
    public int id;
    public string name_kr;
    public EnvironmentType environment_type;

    [Tooltip("연결 상태(0~15)에 따른 벽 스프라이트")]
    public List<Sprite> environmentalSprites = new List<Sprite>(16);

    public Sprite GetSprite(int bitmask)
    {
        return (bitmask >= 0 && bitmask < environmentalSprites.Count) ? environmentalSprites[bitmask] : null;
    }
}
