// RoomPresetEditor.cs (Editor 폴더 안에 위치)
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomPreset))]
public class RoomPresetEditor : Editor
{
    private RoomPreset preset;

    void OnEnable()
    {
        preset = (RoomPreset)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (preset == null || preset.tileInfo == null) return;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        foreach (var kvp in preset.tileInfo)
        {
            Vector2Int pos = kvp.Key;
            TileInfoForRoom tile = kvp.Value;

            // 그리기 위치 설정
            Vector3 drawPos = new Vector3(pos.x, pos.y, 0);

            // 박스 그리기
            Handles.DrawSolidRectangleWithOutline(
                new Vector3[]
                {
                    drawPos + new Vector3(0, 0),
                    drawPos + new Vector3(1, 0),
                    drawPos + new Vector3(1, 1),
                    drawPos + new Vector3(0, 1)
                },
                new Color(0.2f, 1f, 0.2f, 0.2f), // Fill color
                Color.green // Outline color
            );

            // 레이블 표시
            Handles.Label(drawPos + Vector3.up * 0.3f, tile.tileType.ToString(), new GUIStyle()
            {
                fontSize = 10,
                normal = new GUIStyleState() { textColor = Color.white }
            });
        }

        // 씬뷰 갱신
        SceneView.RepaintAll();
    }
}
