using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName ="Recipe")]
public class RecipeData : ScriptableObject
{
    public int id;
    public string recipe_name_kr;
    public int output_item_id;
    public int output_amount;
    public int material_1_item_id;
    public int material_1_count;
    public int material_2_item_id;
    public int material_2_count;
    public int material_3_item_id;
    public int material_3_count;
    public string tags;
    public string unlock_condition;
    public int efficiency_rating;

}
