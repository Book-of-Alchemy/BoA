using System.Collections.Generic;
using UnityEngine;

public enum TileEnvironment
{
    Ground,
    Air,
}

[CreateAssetMenu(menuName = "Tile/EnvironmentalData")]
public class EnvironmentalData : ScriptableObject
{
    public int Id;
    public string name_kr;
    public string name_en;
    public string description;
    public int duration;
    public EnvironmentType environment_type;
    public TileEnvironment tileEnvironment;
    public Material material;
    public List<Sprite> animationSprites = new List<Sprite>();
    [Tooltip("연결 상태(0~15)에 따른 벽 스프라이트")]
    public List<Sprite> environmentalSprites = new List<Sprite>(16);

    public Sprite GetSprite(int bitmask)
    {
        return (bitmask >= 0 && bitmask < environmentalSprites.Count) ? environmentalSprites[bitmask] : null;
    }
}
