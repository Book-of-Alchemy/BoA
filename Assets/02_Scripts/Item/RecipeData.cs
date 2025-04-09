using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName ="Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipe_id;
    public string recipe_name_kr;
    public string output_item_id;
    public int output_amount;
    public string material_1_item_id;
    public int material_1_count;
    public string material_2_item_id;
    public int material_2_count;
    public string material_3_item_id;
    public int material_3_count;
    public string tags;
    public string unlock_condition;
    public int efficiency_rating;

}
