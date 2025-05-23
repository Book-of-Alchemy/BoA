using UnityEngine;
using UnityEngine.UI;

public class RecipeBox : MonoBehaviour
{
    [SerializeField] private Image _materialImage1;
    [SerializeField] private Image _materialImage2;
    [SerializeField] private Image _materialImage3;
    [SerializeField] private Image _plusImage2;
    [Header("결과 이미지")]
    [SerializeField] private Image _resultImage;
    private int _invalidId = -1;

    public void SetRecipe(RecipeData recipeData)
    {
        // 재료 1, 2 설정
        ApplyMaterialImage(recipeData.material_1_item_id, _materialImage1);
        ApplyMaterialImage(recipeData.material_2_item_id, _materialImage2);

        // 재료 3 존재 여부
        bool hasThird = recipeData.material_3_item_id != _invalidId;
        _materialImage3.gameObject.SetActive(hasThird);
        _plusImage2.gameObject.SetActive(hasThird);

        if (hasThird)
            ApplyMaterialImage(recipeData.material_3_item_id, _materialImage3);

        // 결과 아이템 설정
        ApplyResultImage(recipeData.output_item_id, _resultImage);
    }

    private void ApplyMaterialImage(int itemId, Image imageTarget)
    {
        var data = SODataManager.Instance.itemDataBase.GetItemDataById(itemId);
        if (data != null && data.sprite != null)
        {
            imageTarget.sprite = data.sprite;
            imageTarget.color = Color.white;
        }
        else
        {
            imageTarget.sprite = null;
            imageTarget.color = Color.clear;
        }
    }

    private void ApplyResultImage(int itemId, Image imageTarget)
    {
        var data = SODataManager.Instance.itemDataBase.GetItemDataById(itemId);
        if (data != null && data.sprite != null)
        {
            imageTarget.sprite = data.sprite;
            imageTarget.color = Color.white;
        }
        else
        {
            imageTarget.sprite = null;
            imageTarget.color = Color.clear;
        }
    }
}
