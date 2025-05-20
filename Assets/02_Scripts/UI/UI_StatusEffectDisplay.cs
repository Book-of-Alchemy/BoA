using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatusEffectDisplay : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats; // 표시할 대상(보통 플레이어)
    [SerializeField] private GameObject statusEffectIconPrefab; // 상태 효과 아이콘 프리팹
    [SerializeField] private Transform iconsContainer; // 아이콘들을 담을 컨테이너
    [SerializeField] private float iconSpacing = 5f; // 아이콘 간격
    [SerializeField] private int maxIconsPerRow = 6; // 한 줄에 표시할 최대 아이콘 수
    
    private Dictionary<StatusEffect, UI_StatusEffectIcon> activeIconDictionary = new Dictionary<StatusEffect, UI_StatusEffectIcon>();
    
    private void Start()
    {
        if (targetStats == null && GameManager.Instance != null)
        {
            targetStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
        }
        
        // 턴 변경 이벤트 구독
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnGlobalTimeChanged += UpdateAllIcons;
        }
        
        // 초기 상태 표시
        UpdateAllIcons();
    }
    
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnGlobalTimeChanged -= UpdateAllIcons;
        }
    }
    
    // 공개 메서드: 상태 효과 즉시 업데이트
    public void ForceUpdateIcons()
    {
        UpdateAllIcons();
    }
    
    // 상태 표시 타겟 설정 (플레이어가 아닌 대상도 가능)
    public CharacterStats TargetStats
    {
        get => targetStats;
        set 
        {
            if (targetStats != value)
            {
                targetStats = value;
                ClearAllIcons();
                UpdateAllIcons();
            }
        }
    }
    
    // 모든 아이콘 제거
    private void ClearAllIcons()
    {
        foreach (var icon in activeIconDictionary.Values)
        {
            if (icon != null && icon.gameObject != null)
            {
                Destroy(icon.gameObject);
            }
        }
        activeIconDictionary.Clear();
    }
    
    // 모든 상태 효과 업데이트
    private void UpdateAllIcons()
    {
        if (targetStats == null) return;
        
        // 현재 활성화된 모든 상태 효과 확인
        HashSet<StatusEffect> currentEffects = new HashSet<StatusEffect>(targetStats.activeEffects);
        
        // 제거된 상태 효과의 아이콘 정리
        List<StatusEffect> effectsToRemove = new List<StatusEffect>();
        foreach (var kvp in activeIconDictionary)
        {
            if (!currentEffects.Contains(kvp.Key) || kvp.Key.IsExpired)
            {
                effectsToRemove.Add(kvp.Key);
            }
        }
        
        foreach (StatusEffect effect in effectsToRemove)
        {
            RemoveStatusIcon(effect);
        }
        
        // 새로운 상태 효과의 아이콘 추가
        foreach (StatusEffect effect in currentEffects)
        {
            if (!activeIconDictionary.ContainsKey(effect))
            {
                AddStatusIcon(effect);
            }
            else
            {
                // 기존 아이콘 업데이트
                activeIconDictionary[effect].UpdateIcon();
            }
        }
        
        // 아이콘 배치 재조정
        ArrangeIcons();
    }
    
    // 새 상태 효과 아이콘 추가
    private void AddStatusIcon(StatusEffect effect)
    {
        if (effect.data == null) return;
        
        GameObject iconObject = Instantiate(statusEffectIconPrefab, iconsContainer);
        UI_StatusEffectIcon iconComponent = iconObject.GetComponent<UI_StatusEffectIcon>();
        
        iconComponent.Initialize(effect);
        activeIconDictionary.Add(effect, iconComponent);
    }
    
    // 상태 효과 아이콘 제거
    private void RemoveStatusIcon(StatusEffect effect)
    {
        if (activeIconDictionary.TryGetValue(effect, out UI_StatusEffectIcon icon))
        {
            Destroy(icon.gameObject);
            activeIconDictionary.Remove(effect);
        }
    }
    
    // 아이콘들을 그리드 형태로 정렬
    private void ArrangeIcons()
    {
        int index = 0;
        foreach (var icon in activeIconDictionary.Values)
        {
            int row = index / maxIconsPerRow;
            int col = index % maxIconsPerRow;
            
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            float iconWidth = rectTransform.rect.width;
            float iconHeight = rectTransform.rect.height;
            
            rectTransform.anchoredPosition = new Vector2(
                col * (iconWidth + iconSpacing),
                -row * (iconHeight + iconSpacing)
            );
            
            index++;
        }
    }
} 