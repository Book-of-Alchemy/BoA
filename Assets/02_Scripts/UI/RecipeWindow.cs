using System.Collections.Generic;
using UnityEngine;

public class RecipeWindow : MonoBehaviour
{
    [SerializeField] private List<RecipeBox> _recipeBoxes;

    private void Start()
    {
        InitializeRecipes();
    }

    public void InitializeRecipes()
    {
        // 레시피 ID 기준 정렬
        var sortedRecipes = new List<RecipeData>(SODataManager.Instance.itemDataBase.recipeDatas);
        sortedRecipes.Sort((a, b) => a.id.CompareTo(b.id));

        int count = Mathf.Min(_recipeBoxes.Count, sortedRecipes.Count);
        for (int i = 0; i < count; i++)
        {
            _recipeBoxes[i].gameObject.SetActive(true);
            _recipeBoxes[i].SetRecipe(sortedRecipes[i]);
        }

        // 남은 박스는 비활성화
        for (int i = count; i < _recipeBoxes.Count; i++)
        {
            _recipeBoxes[i].gameObject.SetActive(false);
        }
    }
}
