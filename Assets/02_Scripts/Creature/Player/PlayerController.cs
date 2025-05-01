using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStats), typeof(CharacterAnimator))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;

    public PlayerInputActions InputActions { get; private set; }
    public Action onActionConfirmed;
    public bool isPlayerTurn;

    [SerializeField] private GameObject _highlightPrefab;

    private PlayerBaseBehavior currentBehavior;

    private void Awake()
    {
        InputActions = new PlayerInputActions();
    }
    private void Start()
    {
        HandleSceneTypeChanged(GameSceneManager.Instance.CurrentSceneType);//씬 타입에 맞는 Vehavior적용
    }
    private void OnEnable()
    {
        InputActions.PC.Enable();
        GameSceneManager.Instance.OnSceneTypeChanged += HandleSceneTypeChanged;
        GameManager.Instance.RegisterPlayer(GetComponent<PlayerStats>());
    }

    public void OnDisable()
    {
        InputActions.PC.Disable();
        GameSceneManager.Instance.OnSceneTypeChanged -= HandleSceneTypeChanged;
    }

    private void HandleSceneTypeChanged(SceneType sceneType)
    {
        if (sceneType == SceneType.MainMenu) return;

        if (currentBehavior != null)
        {
            InputActions.PC.RemoveCallbacks(currentBehavior);
            Destroy(currentBehavior);       // 컴포넌트만 삭제
        }

        switch (sceneType)
        {
            case SceneType.Dungeon:
                var db = gameObject.AddComponent<DungeonBehavior>();
                db.highlightPrefab = _highlightPrefab;     // 하이라이트 프리펩 할당
                currentBehavior = db;
                break;
            case SceneType.Town:
                currentBehavior = gameObject.AddComponent<TownBehavior>();
                break;
            default:
                Debug.LogError($"Unsupported SceneType: {sceneType}");
                return;
        }
        currentBehavior.Initialize(this);
    }
}