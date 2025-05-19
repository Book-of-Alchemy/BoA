using UnityEngine;

[CreateAssetMenu(menuName = "Research/ResearchData")]
public class ResearchData : ScriptableObject
{
    public int id;
    public string name_kr;
    public string name_en;
    public string description_kr;
    public StatType stat_type;
    public int stat_value;
    public int required_research_id;
    public int research_cost;
    public int max_level;
    public string icon_sprite_id;
    public Sprite icon_sprite;

}
