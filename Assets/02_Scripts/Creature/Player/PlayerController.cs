using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveActionCost = 1f;
    public LayerMask unitLayer;
    public LayerMask obstacleLayer;
    public float inputBufferDuration = 0.1f;
    public int dashDistance = 3;

    private PlayerStats _playerStats;
    private CharacterAnimator _animator;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private PlayerInputActions _inputActions;

    private bool _isMoving;
    private bool _isCtrlHold;

    private Coroutine _moveBufferCoroutine;
    private Coroutine _attackBufferCoroutine;
    private Coroutine _dashBufferCoroutine;
    private Coroutine _dashCoroutine;
    private Coroutine _highlightCoroutine;

    private Vector2 _lastMoveDirection = Vector2.right;
    private Queue<Vector2Int> _dashQueue;
    private float _savedTurnSpeed;
    private float _dashStartHealth;
    private HashSet<EnemyStats> _initialVisibleEnemies;

    [SerializeField] private GameObject _highlightPrefab;
    private GameObject _highlightInstance;


    public event Action onActionConfirmed;
    public bool isPlayerTurn;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _inputActions = InputManager.Instance.GetInputSafe();
    }

    private void OnEnable()
    {
        _inputActions.PC.Move.started += OnMoveStarted;
        _inputActions.PC.Attack.started += OnAttackStarted;
        _inputActions.PC.Dash.started += OnDashStarted;
        _inputActions.PC.Ctrl.performed += OnCtrlPressed;
        _inputActions.PC.Ctrl.canceled += OnCtrlReleased;
    }

    public void OnDisable()
    {
        _inputActions.PC.Move.started -= OnMoveStarted;
        _inputActions.PC.Attack.started -= OnAttackStarted;
        _inputActions.PC.Dash.started -= OnDashStarted;
        _inputActions.PC.Ctrl.performed -= OnCtrlPressed;
        _inputActions.PC.Ctrl.canceled -= OnCtrlReleased;

        if (_moveBufferCoroutine != null) StopCoroutine(_moveBufferCoroutine);
        if (_attackBufferCoroutine != null) StopCoroutine(_attackBufferCoroutine);
        if (_dashBufferCoroutine != null) StopCoroutine(_dashBufferCoroutine);
        if (_highlightCoroutine != null) StopCoroutine(_highlightCoroutine);
    }

    private void OnCtrlPressed(InputAction.CallbackContext ctx)
    {
        _isCtrlHold = true;
        _highlightCoroutine = StartCoroutine(HighlightLoop());
    }

    private void OnCtrlReleased(InputAction.CallbackContext ctx)
    {
        _isCtrlHold = false;
        if (_highlightCoroutine != null) StopCoroutine(_highlightCoroutine);
        HideHighlight();
    }

    private IEnumerator HighlightLoop()
    {
        if (_highlightInstance == null)
            _highlightInstance = Instantiate(_highlightPrefab);
        else
            _highlightInstance.SetActive(true);

        while (_isCtrlHold)
        {
            UpdateHighlightPosition();
            yield return null;
        }

        HideHighlight();
    }

    private void OnMoveStarted(InputAction.CallbackContext ctx)
    {
        if (!isPlayerTurn || _isMoving || _moveBufferCoroutine != null) return;
        _moveBufferCoroutine = StartCoroutine(BufferInput(buf => {
            if (buf != Vector2.zero) ExecuteMove(buf);
            _moveBufferCoroutine = null;
        }, inputBufferDuration));
    }

    private void OnAttackStarted(InputAction.CallbackContext ctx)
    {
        if (!isPlayerTurn || _attackBufferCoroutine != null) return;
        _attackBufferCoroutine = StartCoroutine(BufferInput(buf => {
            _animator.PlayAttack();
            onActionConfirmed?.Invoke();
            _attackBufferCoroutine = null;
        }, inputBufferDuration));
    }

    private void OnDashStarted(InputAction.CallbackContext ctx)
    {
        if (!isPlayerTurn || _dashBufferCoroutine != null) return;
        _dashBufferCoroutine = StartCoroutine(BufferInput(raw => {
            StartDash(raw);
            _dashBufferCoroutine = null;
        }, inputBufferDuration));
    }

    private IEnumerator BufferInput(Action<Vector2> onBuffered, float duration)
    {
        float elapsed = 0f;
        Vector2 buf = Vector2.zero;
        while (elapsed < duration)
        {
            Vector2 current = _inputActions.PC.Move.ReadValue<Vector2>();
            if (current != Vector2.zero) buf = current;
            elapsed += Time.deltaTime;
            yield return null;
        }
        onBuffered(buf);
    }

    private void ExecuteMove(Vector2 rawInput)
    {
        Vector2Int curCell = _playerStats.CurTile.gridPosition;
        Vector2Int offset = new Vector2Int(
            rawInput.x > 0 ? 1 : rawInput.x < 0 ? -1 : 0,
            rawInput.y > 0 ? 1 : rawInput.y < 0 ? -1 : 0);

        if (_isCtrlHold)
        {
            if (offset.x != 0) _spriteRenderer.flipX = offset.x < 0;
            _lastMoveDirection = offset;
            return;
        }

        if (offset.x != 0) _spriteRenderer.flipX = offset.x < 0;
        _lastMoveDirection = offset;

        Vector2Int tgtCell = curCell + offset;
        _spriteRenderer.sortingOrder = -tgtCell.y * 10 + 1;
        if (!_playerStats.curLevel.tiles.TryGetValue(tgtCell, out var tile)
            || tile.CharacterStatsOnTile != null
            || !tile.IsWalkable) return;

        _isMoving = true;
        _playerStats.CurTile.CharacterStatsOnTile = null;
        _playerStats.CurTile = tile;
        tile.CharacterStatsOnTile = _playerStats;

        Vector3 dest = new Vector3(tgtCell.x, tgtCell.y, 0f);
        onActionConfirmed?.Invoke();
        _animator.PlayMove();
        transform.DOMove(dest, 0.1f)
            .SetEase(Ease.Linear)
            .OnComplete(() => _isMoving = false);
    }

    private void StartDash(Vector2 buf)
    {
        Vector2 raw = buf != Vector2.zero ? buf : _lastMoveDirection;
        Vector2Int offset = new Vector2Int(
            raw.x > 0 ? 1 : raw.x < 0 ? -1 : 0,
            raw.y > 0 ? 1 : raw.y < 0 ? -1 : 0);
        _lastMoveDirection = offset;

        Vector2Int startPos = _playerStats.CurTile.gridPosition;
        int maxSteps = 0;
        for (int i = 1; i <= dashDistance; i++)
        {
            Vector2Int next = startPos + offset * i;
            if (!_playerStats.curLevel.tiles.TryGetValue(next, out var t)
                || !t.IsWalkable
                || t.CharacterStatsOnTile != null) break;
            maxSteps = i;
        }
        if (maxSteps <= 0) return;

        _dashStartHealth = _playerStats.CurrentHealth;
        _initialVisibleEnemies = new HashSet<EnemyStats>();
        foreach (var tile in _playerStats.tilesOnVision)
            if (tile.CharacterStatsOnTile is EnemyStats e)
                _initialVisibleEnemies.Add(e);

        var tm = TurnManager.Instance;
        _savedTurnSpeed = tm.turnSpeed;
        tm.turnSpeed *= 10f;
        moveSpeed *= 10f;
        Time.timeScale *= 10f;

        _dashQueue = new Queue<Vector2Int>();
        for (int i = 0; i < maxSteps; i++) _dashQueue.Enqueue(offset);
        _dashCoroutine = StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        bool cancelled = false;
        bool firstStep = true;
        var tm = TurnManager.Instance;

        while (_dashQueue.Count > 0 && !cancelled)
        {
            if (!firstStep) yield return new WaitUntil(() => isPlayerTurn);
            firstStep = false;

            Vector2Int step = _dashQueue.Dequeue();
            var cur = _playerStats.CurTile.gridPosition;
            var next = cur + step;

            if (!_playerStats.curLevel.tiles.TryGetValue(next, out var tile)
                || !tile.IsWalkable
                || tile.CharacterStatsOnTile != null
                || _playerStats.CurrentHealth < _dashStartHealth
                || (tile.TrpaOnTile != null && tile.TrpaOnTile.IsDetected)) break;

            foreach (var vis in _playerStats.tilesOnVision)
            {
                if (vis.CharacterStatsOnTile is EnemyStats e && !_initialVisibleEnemies.Contains(e))
                {
                    cancelled = true;
                    break;
                }
            }

            if (step.x != 0) _spriteRenderer.flipX = step.x < 0;
            _animator.PlayMove();
            _playerStats.CurTile.CharacterStatsOnTile = null;
            _playerStats.CurTile = tile;
            tile.CharacterStatsOnTile = _playerStats;
            Vector3 dest = new Vector3(next.x, next.y, 0f);
            yield return transform.DOMove(dest, 0.01f)
                .SetEase(Ease.Linear)
                .WaitForCompletion();

            foreach (var vis in _playerStats.tilesOnVision)
            {
                if (vis.CharacterStatsOnTile is EnemyStats e && !_initialVisibleEnemies.Contains(e))
                {
                    cancelled = true;
                    break;
                }
            }

            onActionConfirmed?.Invoke();
        }

        yield return new WaitUntil(() => isPlayerTurn);
        tm.turnSpeed = _savedTurnSpeed;
        moveSpeed /= 10f;
        Time.timeScale /= 10f;
        _dashCoroutine = null;
    }

    public void OnAttackHit() => DoAttack(_lastMoveDirection);

    private void DoAttack(Vector2 dir)
    {
        if (dir.x != 0) _spriteRenderer.flipX = dir.x < 0;
        PerformAttackRaycast(dir);
    }

    private void PerformAttackRaycast(Vector2 dir)
    {
        onActionConfirmed?.Invoke();
        float originOffset = _collider.bounds.extents.magnitude + 0.1f;
        Vector2 origin = (Vector2)transform.position + dir.normalized * originOffset;
        Vector2 direction = dir.normalized;

        Debug.DrawRay(origin, direction * 0.5f, Color.red, 0.5f);
        var hit = Physics2D.Raycast(origin, direction, 0.5f, unitLayer);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            var enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats != null) _playerStats.Attack(enemyStats);
        }
    }

    private void HideHighlight()
    {
        if (_highlightInstance != null)
            _highlightInstance.SetActive(false);
    }

    private void UpdateHighlightPosition()
    {
        Vector2Int cur = _playerStats.CurTile.gridPosition;
        Vector2Int dir = new Vector2Int(
            _lastMoveDirection.x > 0 ? 1 : _lastMoveDirection.x < 0 ? -1 : 0,
            _lastMoveDirection.y > 0 ? 1 : _lastMoveDirection.y < 0 ? -1 : 0);
        Vector2Int target = cur + dir;

        if (!_playerStats.curLevel.tiles.TryGetValue(target, out var tile))
        {
            if (_highlightInstance != null)
                _highlightInstance.SetActive(false);
            return;
        }

        _highlightInstance.SetActive(true);
        Vector3 worldPos = new Vector3(target.x, target.y, 0f);
        _highlightInstance.transform.position = worldPos;
        var sr = _highlightInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            int y = target.y;
            if (y == 0)
                sr.sortingOrder = -y - 1;
            else if (y > 0)
                sr.sortingOrder = -y * 11;
            else
                sr.sortingOrder = -y * 9;
        }
    }
}
