using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance { get; private set; }

    [Header("UI 요소")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private GameObject loadingIcon;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("로딩 설정")]
    [SerializeField] private string[] tipMessages;
    [SerializeField] private float iconRotationSpeed = 150f;       // 초당 회전 각도
    [SerializeField] private float progressTweenDuration = 0.5f;   // 프로그레스 바 트윈 지속 시간
    [SerializeField] private float fadeInSpeed = 1.0f;            // 페이드인 속도

    private Coroutine _tipRoutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        InitializeUI();
        FadeIn();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ChangeTipMessage();
    }

    private void InitializeUI()
    {
        // 초기 팁 표시 및 자동 변경 코루틴 시작
        ChangeTipMessage();
        if (_tipRoutine != null)
            StopCoroutine(_tipRoutine);
        _tipRoutine = StartCoroutine(ChangeTipMessageRoutine());

        // 로딩 아이콘 회전 시작
        if (loadingIcon != null)
            StartCoroutine(RotateLoadingIcon());

        // 프로그레스 바 초기화
        if (progressBar != null)
        {
            progressBar.value = 0;
            if (progressText != null)
                progressText.text = "0%";
        }
    }

    private void ChangeTipMessage()
    {
        if (tipText == null || tipMessages == null || tipMessages.Length == 0)
            return;

        int randomIndex = Random.Range(0, tipMessages.Length);
        tipText.text = tipMessages[randomIndex];
    }

    private IEnumerator ChangeTipMessageRoutine()
    {
        var waitTime = new WaitForSeconds(10f);
        while (true)
        {
            yield return waitTime;
            ChangeTipMessage();
        }
    }

    /// <summary>
    /// DOTween을 사용해 프로그레스 바 값을 트윈으로 업데이트
    /// </summary>
    public void UpdateProgress(float progress)
    {
        if (progressBar == null)
            return;

        progressBar.DOValue(progress, progressTweenDuration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                if (progressText != null)
                    progressText.text = $"{Mathf.RoundToInt(progressBar.value * 100)}%";
            });
    }

    private IEnumerator RotateLoadingIcon()
    {
        while (true)
        {
            if (loadingIcon != null)
                loadingIcon.transform.Rotate(0, 0, -iconRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void FadeIn()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup이 없습니다!");
            return;
        }

        canvasGroup.alpha = 0f;
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float targetAlpha = 1.0f;
        float currentAlpha = canvasGroup.alpha;
        while (currentAlpha < targetAlpha)
        {
            currentAlpha += Time.deltaTime * fadeInSpeed;
            canvasGroup.alpha = currentAlpha;
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
