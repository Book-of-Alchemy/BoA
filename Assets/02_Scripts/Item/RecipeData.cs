
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName ="Recipe")]
public class RecipeData : ScriptableObject
{
    public int id;
    public string recipe_name_kr;
    public string recipe_name_en;
    public int output_item_id;
    public int output_amount;
    public int material_1_item_id;
    public int material_1_amount;
    public int material_2_item_id;
    public int material_2_amount;
    public int material_3_item_id;
    public int material_3_amount;
    public int mp_cost;
    //public string tags;
    public string unlock_condition;
    public int efficiency_rating;
    public string icon_sprite_id;

}
