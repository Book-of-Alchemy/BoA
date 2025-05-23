
using UnityEngine;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
}

[CreateAssetMenu(fileName = "ArtifactData",menuName = "Artifact/Artifact")]
public class ArtifactData : ScriptableObject
{
    public int id;
    public string name_kr;
    public string name_en;
    public string description;
    public string icon_sprite_id;
    public Sprite icon_sprite;
    public Rarity rarity;
}
