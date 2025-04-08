using UnityEngine;
using UnityEngine.UI;

//
[RequireComponent(typeof(Scrollbar))]
public class CustomSizeScrollbar : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scrollRect;
    private Scrollbar scrollbar;

    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();

        if (scrollRect == null)
            return;

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
