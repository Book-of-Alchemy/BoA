// DungeonBehavior.cs

using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStats), typeof(CharacterAnimator), typeof(SpriteRenderer))]
public class DungeonBehavior : PlayerBaseBehavior
{
    [Header("Dungeon Settings")]
    [SerializeField] private float _inputBufferDuration = 0.1f;
    public GameObject highlightPrefab;

    // Components & 상태
    private PlayerStats _stats;
    private CharacterAnimator _animator;
    private SpriteRenderer _spriteRenderer;

    // 상태 플래그
    private bool _isMoving;
    private bool _isCtrlHeld;
    private bool _isDashHeld;

    // 코루틴 핸들
    private Coroutine _moveBuffer;
    private Coroutine _attackBuffer;
    private Coroutine _highlightBuffer;

    // 마지막 이동 방향 (하이라이트용)
    private Vector2Int _lastMoveDir = Vector2Int.right;

    // 달리기(Shift) 상태 저장
    private float _savedTurnSpeed;

    // 하이라이트 인스턴스
    private GameObject _highlightInstance;

    // 아이템 사용
    private BaseItem _currentItem;

    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);

        _stats = GetComponent<PlayerStats>();
        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        SubscribeInput();
    }

    protected override void SubscribeInput()
    {
        InputManager.OnMove += HandleMove;
        InputManager.OnAttack += HandleAttack;
        InputManager.OnDashStart += HandleDashStart;   // Shift 누름
        InputManager.OnDashEnd += HandleDashEnd;     // Shift 뗌
        InputManager.OnCtrlStart += HandleCtrlStart;
        InputManager.OnCtrlEnd += HandleCtrlEnd;
        InputManager.OnInteract += HandleInteract;
    }

    protected override void UnsubscribeInput()
    {
        InputManager.OnMove -= HandleMove;
        InputManager.OnAttack -= HandleAttack;
        InputManager.OnDashStart -= HandleDashStart;
        InputManager.OnDashEnd -= HandleDashEnd;
        InputManager.OnCtrlStart -= HandleCtrlStart;
        InputManager.OnCtrlEnd -= HandleCtrlEnd;
        InputManager.OnInteract -= HandleInteract;
    }

    // ────────────────────────────────────────────────────────
    // 1) 공통 버퍼 입력 코루틴
    private IEnumerator BufferInput(Action<Vector2Int> onBuffered)
    {
        float elapsed = 0f;
        Vector2 rawBuf = InputManager.MoveInput;

        while (elapsed < _inputBufferDuration)
        {
            Vector2 raw = InputManager.MoveInput;
            if (raw.x != 0) rawBuf.x = raw.x;
            if (raw.y != 0) rawBuf.y = raw.y;

            elapsed += Time.deltaTime;
            yield return null;
        }

        var dir = new Vector2Int(
            rawBuf.x > 0 ? 1 : rawBuf.x < 0 ? -1 : 0,
            rawBuf.y > 0 ? 1 : rawBuf.y < 0 ? -1 : 0
        );

        if (dir != Vector2Int.zero)
            onBuffered(dir);
    }

    // ────────────────────────────────────────────────────────
    // 2) 이동 처리
    private void HandleMove(Vector2 raw)
    {
        if (!Controller.isPlayerTurn ||
            _isMoving ||
            _moveBuffer != null ||
            raw == Vector2.zero)
            return;

        _moveBuffer = StartCoroutine(BufferInput(dir =>
        {
            ExecuteMove(dir);
            _moveBuffer = null;
        }));
    }

    private void ExecuteMove(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return;

        _lastMoveDir = dir;
        if (dir.x != 0)
            _spriteRenderer.flipX = dir.x < 0;

        if (_isCtrlHeld) return;

        var cur = _stats.CurTile.gridPosition;
        var nxt = cur + dir;
        if (!_stats.curLevel.tiles.TryGetValue(nxt, out var tile) ||
            !tile.IsWalkable ||
             tile.CharacterStatsOnTile != null)
            return;

        _isMoving = true;
        _stats.CurTile.CharacterStatsOnTile = null;
        _stats.CurTile = tile;
        tile.CharacterStatsOnTile = _stats;

        _spriteRenderer.sortingOrder = -nxt.y * 10 + 1;
        _animator.PlayMove();

        transform.DOMove(new Vector3(nxt.x, nxt.y, 0), 0.1f)
            .SetEase(Ease.Linear)
            .OnComplete(() => _isMoving = false);

        Controller.onActionConfirmed?.Invoke();
    }

    // ────────────────────────────────────────────────────────
    // 3) 공격 처리
    private void HandleAttack()
    {
        if (!Controller.isPlayerTurn || _attackBuffer != null) return;
        _attackBuffer = StartCoroutine(BufferAttack());
    }

    private IEnumerator BufferAttack()
    {
        yield return new WaitForSeconds(_inputBufferDuration);
        _animator.PlayAttack();
        Controller.onActionConfirmed?.Invoke();
        _attackBuffer = null;
    }

    // ────────────────────────────────────────────────────────
    // 4) Shift 누름 시: 타임스케일 증가
    private void HandleDashStart()
    {
        if (_isDashHeld) return;
        _isDashHeld = true;

        var tm = TurnManager.Instance;
        _savedTurnSpeed = tm.turnSpeed;
        tm.turnSpeed *= 10;
        Controller.moveSpeed *= 10;
        Time.timeScale *= 10;
    }

    // ────────────────────────────────────────────────────────
    // 5) Shift 해제 시: 원상 복구
    private void HandleDashEnd()
    {
        if (!_isDashHeld) return;
        _isDashHeld = false;

        var tm = TurnManager.Instance;
        tm.turnSpeed = _savedTurnSpeed;
        Controller.moveSpeed /= 10;
        Time.timeScale /= 10;
    }

    // ────────────────────────────────────────────────────────
    // 6) Ctrl + 하이라이트
    private void HandleCtrlStart()
    {
        _isCtrlHeld = true;

        if (_highlightInstance == null)
            _highlightInstance = Instantiate(highlightPrefab);

        _highlightInstance.SetActive(true);
        UpdateHighlightPosition();
        _highlightBuffer = StartCoroutine(HighlightWithBuffer());
    }

    private void HandleCtrlEnd()
    {
        _isCtrlHeld = false;
        if (_highlightBuffer != null)
            StopCoroutine(_highlightBuffer);
        HideHighlight();
    }

    private IEnumerator HighlightWithBuffer()
    {
        while (_isCtrlHeld)
        {
            yield return BufferInput(dir =>
            {
                _lastMoveDir = dir;
                UpdateHighlightPosition();
            });
        }
        HideHighlight();
    }

    private void UpdateHighlightPosition()
    {
        var cur = _stats.CurTile.gridPosition;
        var tgt = cur + _lastMoveDir;

        if (!_stats.curLevel.tiles.TryGetValue(tgt, out var tile) ||
            !tile.IsWalkable)
        {
            _highlightInstance.SetActive(false);
            return;
        }

        _highlightInstance.SetActive(true);
        _highlightInstance.transform.position = new Vector3(tgt.x, tgt.y, 0);

        var sr = _highlightInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            int y = tgt.y;
            sr.sortingOrder = y == 0
                ? -1
                : (y > 0 ? -y * 11 : -y * 9);
        }
    }

    private void HideHighlight()
    {
        if (_highlightInstance != null)
            _highlightInstance.SetActive(false);
    }

    // ────────────────────────────────────────────────────────
    // 7) 상호작용 & 아이템 사용
    private void HandleInteract()
    {
        // 필요 시 구현
    }

    public void UseItem(ItemData data)
    {
        _currentItem = ItemFactory.Instance.CreateItem(data.id);
        if (_currentItem == null) return;

        _currentItem.ItemUseDone += HandleItemUseDone;
        _currentItem.UseItem(data);
    }

    private void HandleItemUseDone()
    {
        if (_currentItem == null) return;

        _currentItem.ItemUseDone -= HandleItemUseDone;
        Controller.onActionConfirmed?.Invoke();
        _currentItem = null;
    }
}
