using UnityEditor;
using UnityEngine;

public class SpritePivotBatchSetter : EditorWindow
{
    private Texture2D spriteSheet;
    private Vector2 pivot = new Vector2(0.5f, 0.5f);

    [MenuItem("Tools/Sprite Pivot Batch Setter")]
    static void Init()
    {
        GetWindow<SpritePivotBatchSetter>("Pivot Setter");
    }

    void OnGUI()
    {
        spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet", spriteSheet, typeof(Texture2D), false);
        pivot = EditorGUILayout.Vector2Field("Pivot (0~1)", pivot);

        if (GUILayout.Button("Apply Pivot"))
        {
            ApplyPivot();
        }
    }

    void ApplyPivot()
    {
        if (spriteSheet == null)
        {
            Debug.LogError("스프라이트 시트를 지정해 주세요.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(spriteSheet);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer != null && importer.spriteImportMode == SpriteImportMode.Multiple)
        {
            var sprites = importer.spritesheet;

            for (int i = 0; i < sprites.Length; i++)
            {
                // 강제로 다른 값 주기
                sprites[i].pivot = pivot + new Vector2(0.001f, 0.001f);
                sprites[i].pivot = pivot; // 원하는 값으로 되돌리기
            }

            importer.spritesheet = sprites;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log("Pivot 일괄 변경 완료!");
        }
        else
        {
            Debug.LogError("해당 텍스처가 Multiple 스프라이트가 아닙니다.");
        }
    }
}
