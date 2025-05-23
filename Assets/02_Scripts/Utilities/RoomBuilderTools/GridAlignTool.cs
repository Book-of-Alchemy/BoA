using UnityEditor;
using UnityEngine;

public class GridAlignTool : EditorWindow
{
    private int columns = 5;              // 열 수
    private float cellSize = 1f;          // 셀 간격
    private Vector2 startPosition = Vector2.zero;

    [MenuItem("Tools/Grid Align Tool")]
    public static void ShowWindow()
    {
        GetWindow<GridAlignTool>("Grid Align");
    }

    private void OnGUI()
    {
        GUILayout.Label("선택된 오브젝트를 2D 그리드로 정렬", EditorStyles.boldLabel);

        columns = EditorGUILayout.IntField("열 수 (가로)", columns);
        cellSize = EditorGUILayout.FloatField("셀 크기", cellSize);
        startPosition = EditorGUILayout.Vector2Field("시작 위치", startPosition);

        if (GUILayout.Button("정렬하기"))
        {
            AlignSelectedObjects();
        }
    }

    private void AlignSelectedObjects()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected.Length == 0)
        {
            return;
        }

        // 정렬을 위해 이름순 정렬
        System.Array.Sort(selected, (a, b) => a.name.CompareTo(b.name));

        for (int i = 0; i < selected.Length; i++)
        {
            int x = i % columns;
            int y = i / columns;

            Vector3 newPos = new Vector3(
                startPosition.x + x * cellSize,
                startPosition.y - y * cellSize, // 아래로 정렬 (y 감소)
                0f
            );

            Undo.RecordObject(selected[i].transform, "Grid Align");
            selected[i].transform.position = newPos;
        }

    }
}
