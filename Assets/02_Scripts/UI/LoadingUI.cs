using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance { get; private set; }

    [Header("UI 요소")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject loadingIcon;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("로딩 설정")]
    [SerializeField] private string[] tipMessages;
    [SerializeField] private float iconRotationSpeed = 150f; // 초당 회전 각도
    [SerializeField] private float progressBarSmoothing = 5f; // 프로그레스 바 부드러움 정도
    [SerializeField] private float fadeInSpeed = 1.0f; // 페이드인 속도
    
    private float _targetProgress = 0f;
    private float _currentDisplayProgress = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        // CanvasGroup 컴포넌트 자동 연결
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    private void Start()
    {
        InitializeUI();
        // 시작할 때 자동 페이드인
        FadeIn();
    }
    
    private void Update()
    {
        // 프로그레스 바 부드럽게 업데이트
        if (Mathf.Abs(_currentDisplayProgress - _targetProgress) > 0.01f)
        {
            _currentDisplayProgress = Mathf.Lerp(_currentDisplayProgress, _targetProgress, 
                Time.deltaTime * progressBarSmoothing);
            
            // 슬라이더와 텍스트 업데이트
            UpdateUI(_currentDisplayProgress);
        }
    }

    private void InitializeUI()
    {
        // 진행 바 초기화
        if (progressBar != null)
        {
            progressBar.value = 0;
        }

        // 진행 텍스트 초기화
        if (progressText != null)
        {
            progressText.text = "0%";
        }

        // 랜덤 팁 메시지 선택
        if (tipText != null && tipMessages != null && tipMessages.Length > 0)
        {
            int randomIndex = Random.Range(0, tipMessages.Length);
            tipText.text = tipMessages[randomIndex];
            
            // 10초마다 팁 메시지 변경
            StartCoroutine(ChangeTipMessageRoutine());
        }

        // 로딩 아이콘 회전 애니메이션 시작
        if (loadingIcon != null)
        {
            StartCoroutine(RotateLoadingIcon());
        }
    }
    
    private IEnumerator ChangeTipMessageRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(10f);
        
        while (true)
        {
            yield return waitTime;
            
            if (tipText != null && tipMessages != null && tipMessages.Length > 0)
            {
                int randomIndex = Random.Range(0, tipMessages.Length);
                tipText.text = tipMessages[randomIndex];
            }
        }
    }

    public void UpdateProgress(float progress)
    {
        // 목표 진행률 설정 (Update에서 부드럽게 적용됨)
        _targetProgress = progress;
    }
    
    private void UpdateUI(float progress)
    {
        // 진행 바 업데이트
        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        // 진행 텍스트 업데이트
        if (progressText != null)
        {
            progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }
    }

    private IEnumerator RotateLoadingIcon()
    {
        while (true)
        {
            if (loadingIcon != null)
            {
                // 델타타임을 사용하여 회전 속도 일정하게 유지
                loadingIcon.transform.Rotate(0, 0, -iconRotationSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }
    
    // 페이드인 효과 시작
    public void FadeIn()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            StartCoroutine(FadeInRoutine());
        }
        else
        {
            Debug.LogError("CanvasGroup이 없습니다!");
        }
    }

    // 페이드인 코루틴
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
        {
            Instance = null;
        }
    }
} 