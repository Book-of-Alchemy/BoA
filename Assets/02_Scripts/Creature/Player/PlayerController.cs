using System;
using UnityEngine;

[RequireComponent(typeof(PlayerStats), typeof(CharacterAnimator))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 10f;

    /// <summary>행동이 확정되면 호출됩니다.</summary>
    public Action onActionConfirmed;
    /// <summary>행동이 종료되면 호출됩니다.</summary>
    //public Action onActionPerformed;
    /// <summary>현재 플레이어 차례 여부</summary>
    public bool isPlayerTurn;

    [SerializeField] private GameObject _highlightPrefab;

    private PlayerBaseBehavior currentBehavior;

    private void Start()
    {
        // 시작 시 현재 씬 타입에 맞는 Behavior 생성
        HandleSceneTypeChanged(GameSceneManager.Instance.CurrentSceneType);
    }

    private void OnEnable()
    {
        // InputManager 활성화
        InputManager.Instance.OnEnable();
        // 씬 전환 콜백 등록
        GameSceneManager.Instance.OnSceneTypeChanged += HandleSceneTypeChanged;
        // GameManager에 플레이어 등록
        GameManager.Instance.RegisterPlayer(GetComponent<PlayerStats>());
    }

    public void OnDisable()
    {
        // InputManager 비활성화
        InputManager.Instance.OnDisable();
        // 씬 전환 콜백 해제
        GameSceneManager.Instance.OnSceneTypeChanged -= HandleSceneTypeChanged;
    }

    private void HandleSceneTypeChanged(SceneType sceneType)
    {
        if (sceneType == SceneType.MainMenu)
            return;

        // 이전 Behavior 제거 (이 과정에서 OnDisable() → UnsubscribeInput() 자동 호출)
        if (currentBehavior != null)
        {
            Destroy(currentBehavior);
            currentBehavior = null;
        }

        // 새 Behavior 추가
        switch (sceneType)
        {
            case SceneType.Dungeon:
                var db = gameObject.AddComponent<DungeonBehavior>();
                db.highlightPrefab = _highlightPrefab;
                currentBehavior = db;
                break;

            case SceneType.Town:
                currentBehavior = gameObject.AddComponent<TownBehavior>();
                break;

            default:
                Debug.LogError($"Unsupported SceneType: {sceneType}");
                return;
        }

        // Behavior 초기화 (Controller 참조 전달 및 SubscribeInput 호출)
        currentBehavior.Initialize(this);
    }
}
