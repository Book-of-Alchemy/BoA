using UnityEngine;
using System;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
//CanvasGroup을 통해 하위 요소들을 Animation 제어 하는 클래스
public class UIAnimator : MonoBehaviour
{
    [Header("애니메이션 설정")]
    [Tooltip("나타나고 사라지는 시간")]
    [SerializeField] private float fadeDuration = 0.4f;
    [Tooltip("Slide할 X축 거리")]
    [SerializeField] private float slideOffsetX = 300f;
    [SerializeField] private float slideOffsetY = 300f;
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private float popupScale = 1.1f;
    [SerializeField] private float popupDuration = 0.3f;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    private void Awake()
    {
        if (canvasGroup == null)
            TryGetComponent<CanvasGroup>(out canvasGroup);
        if (rectTransform == null)
            TryGetComponent<RectTransform>(out rectTransform);
    }

    //점차 등장 애니메이션
    //Action을 인자로 받아서 종료시점 필요한 동작이 있다면 처리
    public void FadeIn(Action onComplete = null) 
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, fadeDuration).OnComplete(() => onComplete?.Invoke());
        gameObject.SetActive(true);
    }

    //점차 퇴장 애니메이션
    public void FadeOut(Action onComplete = null) 
    {
        canvasGroup.DOFade(0f, fadeDuration).OnComplete(()=>gameObject.SetActive(false))
            .OnComplete(() => onComplete?.Invoke());
       
    }

    //Offset에서 슬라이드
    public void SlideFrom(Action onComplete = null)
    {
        Vector2 origin = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(slideOffsetX, origin.y);
        gameObject.SetActive(true);

        rectTransform.DOAnchorPosX(origin.x, slideDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => onComplete?.Invoke());
    }

    //Offset으로 슬라이드
    public void SlideTo( Action onComplete = null)
    {
        gameObject.SetActive(true);
        rectTransform.DOAnchorPosX(slideOffsetX, slideDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => onComplete?.Invoke());
    }
    public void SlideFromY(Action onComplete = null)
    {
        Vector2 origin = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(origin.x, slideOffsetY);
        gameObject.SetActive(true);

        rectTransform.DOAnchorPosY(origin.y, slideDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => onComplete?.Invoke());
    }

    //팝업 등장하는 애니메이션
    public void PopUp(Action onComplete = null)
    {
        gameObject.SetActive(true);
        transform.DOScale(popupScale, fadeDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => transform.DOScale(Vector3.one, popupDuration).OnComplete(() => onComplete?.Invoke()));
    }
}
