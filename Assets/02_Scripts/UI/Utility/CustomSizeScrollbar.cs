using UnityEngine;
using UnityEngine.UI;

//ScrollRect으로부터 ScrollBar를 떼어 독립적으로 스크롤하는 스크립트
[RequireComponent(typeof(Scrollbar))]
public class CustomSizeScrollbar : MonoBehaviour
{
    [SerializeField] [Tooltip("조작하려는 ScrollView")]
    private ScrollRect scrollRect; //현재 스크롤바로 조작하려는 ScrollView
    private Scrollbar scrollbar; //필수적으로 필요한 스크롤바 Component

    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();

        if (scrollRect == null)
            return;

        //각각의 이벤트에 서로를 움직이는 메서드 등록
        scrollRect.onValueChanged.AddListener(OnScrollRectPosChanged);
        scrollbar.onValueChanged.AddListener(OnScrollValueChanged);
    }

    void OnScrollRectPosChanged(Vector2 value) 
    {
        float percent = Mathf.Clamp(scrollRect.verticalNormalizedPosition, 0, 1);
        scrollbar.value = percent;
    }
    public void OnScrollValueChanged(float value)
    {
        float percent = Mathf.Clamp(value, 0, 1);
        scrollRect.verticalNormalizedPosition= percent;
    }
}
