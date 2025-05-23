using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_StatusEffectIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private StatusEffect statusEffect;
    private Sequence blinkSequence;
    private bool isBlinking = false;

    [Header("깜빡임 설정")]
    [SerializeField] private float blinkDuration = 0.5f;
    [SerializeField] private float blinkMinAlpha = 0.3f;
    [SerializeField] private Color originalColor = Color.white;
    [SerializeField] private Color warningColor = new Color(1f, 0.3f, 0.3f);

    private void Awake()
    {
        // Image 컴포넌트 찾기
        if (iconImage == null)
        {
            iconImage = GetComponentInChildren<Image>();
            if (iconImage == null)
            {
                Debug.LogWarning("UI_StatusEffectIcon: 자식 오브젝트에서 Image 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }

    public void Initialize(StatusEffect effect)
    {
        if (effect == null || effect.data == null) return;
        statusEffect = effect;

        if (iconImage == null) return;

        // 아이콘 설정
        if (effect.data.icon != null)
        {
            iconImage.sprite = effect.data.icon;
        }
        else
        {
            // icon_sprite에서 "icon_" 접두사를 제거
            // 첫 글자를 대문자로 변환
            string iconName = effect.data.icon_sprite;
            if (iconName.StartsWith("icon_"))
            {
                iconName = iconName.Substring(5); // "icon_" 제거
            }
            
            // 첫 글자를 대문자로 변환
            if (iconName.Length > 0)
            {
                iconName = char.ToUpper(iconName[0]) + iconName.Substring(1);
            }
            
            Debug.Log($"상태 효과 아이콘 로드 시도: {iconName}, 원본 이름: {effect.data.icon_sprite}");
            
            Sprite iconSprite = Resources.Load<Sprite>($"Image/Bufficon/{iconName}");
            if (iconSprite != null)
            {
                iconImage.sprite = iconSprite;
                Debug.Log($"상태 효과 아이콘 로드 성공: {iconName}");
            }
            else
            {
                Debug.LogWarning($"상태 효과 아이콘을 찾을 수 없음: {iconName}");
                
                // 두 번째 시도: 원본 이름 그대로 시도
                iconSprite = Resources.Load<Sprite>($"Image/Bufficon/{effect.data.icon_sprite}");
                if (iconSprite != null)
                {
                    iconImage.sprite = iconSprite;
                    Debug.Log($"원본 이름으로 상태 효과 아이콘 로드 성공: {effect.data.icon_sprite}");
                }
                else
                {
                    Debug.LogError($"모든 방법으로 상태 효과 아이콘을 찾을 수 없음: {effect.data.icon_sprite}");
                }
            }
        }
    }

    public void UpdateIcon()
    {
        if (statusEffect == null) return;

        // 한 턴(10) 이하로 남았을 때 깜빡임 효과 시작
        if (statusEffect.remainingTime <= 10 && !isBlinking)
        {
            StartBlinking();
        }
        else if (statusEffect.remainingTime > 10 && isBlinking)
        {
            StopBlinking();
        }
    }

    private void StartBlinking()
    {
        if (iconImage == null) return;

        isBlinking = true;

        // 기존 깜빡임 제거
        if (blinkSequence != null)
        {
            blinkSequence.Kill();
        }

        // 색상 변경
        iconImage.color = warningColor;

        // 새 깜빡임 생성
        blinkSequence = DOTween.Sequence()
            .Append(iconImage.DOFade(blinkMinAlpha, blinkDuration / 2))
            .Append(iconImage.DOFade(1f, blinkDuration / 2))
            .SetLoops(-1); // 무한 반복
    }

    private void StopBlinking()
    {
        if (iconImage == null) return;

        isBlinking = false;

        if (blinkSequence != null)
        {
            blinkSequence.Kill();
            blinkSequence = null;
        }

        // 원래 색상 및 투명도로 복원
        iconImage.color = originalColor;
        iconImage.DOFade(1f, 0.2f);
    }

    private void OnDestroy()
    {
        if (blinkSequence != null)
        {
            blinkSequence.Kill();
        }
    }
}