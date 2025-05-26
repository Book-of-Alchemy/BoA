
//using UnityEditor;
//using UnityEngine;


//[ExecuteInEditMode]
//public class SceneNote : MonoBehaviour
//{
//    [TextArea(3, 10)]
//    public string note = "이곳에 메모를 입력하세요\n줄바꿈도 됩니다!";

//    private void OnDrawGizmos()
//    {
//        GUIStyle style = new GUIStyle();
//        style.normal.textColor = Color.yellow;
//        style.fontSize = 14;
//        style.wordWrap = true; // 줄바꿈 활성화!

//        // 라벨 위치
//        Vector3 labelPosition = transform.position + Vector3.up * 2;

//        // 라벨 박스 크기 설정
//        Vector2 size = new Vector2(200, 100);

//        // GUI 영역 설정
//        Handles.BeginGUI();
//        Vector3 screenPos = HandleUtility.WorldToGUIPoint(labelPosition);
//        Rect rect = new Rect(screenPos.x, screenPos.y, size.x, size.y);
//        GUI.Label(rect, note, style);
//        Handles.EndGUI();
//    }
//}
