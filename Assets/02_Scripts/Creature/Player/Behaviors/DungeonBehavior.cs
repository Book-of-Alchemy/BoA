using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(PlayerStats), typeof(CharacterAnimator), typeof(SpriteRenderer))]
public class DungeonBehavior : PlayerBaseBehavior
{
    [Header("Dungeon Settings")]
    [SerializeField] private float _inputBufferDuration = 0.1f;
    [SerializeField] private int _dashDistance = 3;
    public GameObject highlightPrefab;

    private PlayerStats _stats;
    private CharacterAnimator _animator;
    private SpriteRenderer _spriteRenderer;

    public bool canMove = true;
    private bool _isMoving;
    private bool _isCtrlHeld;

    private Coroutine _moveBuffer;
    private Coroutine _attackBuffer;
    private Coroutine _dashBuffer;
    private Coroutine _dashCoroutine;
    private Coroutine _highlightCoroutine;

    private Vector2Int _lastMoveDir = Vector2Int.right;
    private Queue<Vector2Int> _dashQueue;

    private float _savedTurnSpeed;
    private float _dashStartHealth;

    private HashSet<EnemyStats> _initialEnemies;
    private GameObject _highlightInstance;
    private BaseItem _currentItem;

    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);

        _stats = GetComponent<PlayerStats>();
        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnMove(InputAction.CallbackContext ctx)
    {
        if (!Controller.isPlayerTurn || _isMoving || _moveBuffer != null || !ctx.started) return;
        if(!canMove)
        {
            Shake();
            return;
        }
        _moveBuffer = StartCoroutine(BufferMove());
    }

    void Shake()
    {
        transform.DOShakePosition(
            duration: 0.2f,
            strength: new Vector3(0.2f, 0, 0),  // 좌우 흔들림 세기 (X축만)
            vibrato: 30,            // 진동 횟수
            randomness: 0,
            snapping: false,
            fadeOut: true           // 점점 작아지게
        );
    }

    private IEnumerator BufferMove()
    {
        float time = 0f;
        Vector2 bufferedDir = Vector2.zero;

        while (time < _inputBufferDuration)
        {
            Vector2 inputDir = InputActions.PC.Move.ReadValue<Vector2>();
            if (inputDir != Vector2.zero)
                bufferedDir = inputDir;

            time += Time.deltaTime;
            yield return null;
        }

        ExecuteMove(bufferedDir);
        _moveBuffer = null;
    }

    private void ExecuteMove(Vector2 rawDir)
    {
        if (rawDir == Vector2.zero) return;

        Vector2Int dir = new(
            rawDir.x > 0 ? 1 : rawDir.x < 0 ? -1 : 0,
            rawDir.y > 0 ? 1 : rawDir.y < 0 ? -1 : 0
        );

        if (dir != Vector2Int.zero)
        {
            _lastMoveDir = dir;
            if (dir.x != 0)
                _spriteRenderer.flipX = dir.x < 0;
        }

        if (_isCtrlHeld) return;

        Vector2Int cur = _stats.CurTile.gridPosition;
        Vector2Int tgt = cur + dir;

        if (!_stats.curLevel.tiles.TryGetValue(tgt, out Tile tile) || !tile.IsWalkable || tile.CharacterStatsOnTile != null)
            return;

        _isMoving = true;
        _stats.CurTile.CharacterStatsOnTile = null;
        _stats.CurTile = tile;
        tile.CharacterStatsOnTile = _stats;

        _spriteRenderer.sortingOrder = -tgt.y * 10 + 1;
        _animator.PlayMove();

        transform.DOMove(new Vector3(tgt.x, tgt.y, 0), 0.1f)
            .SetEase(Ease.Linear)
            .OnComplete(() => _isMoving = false);

        Controller.onActionConfirmed?.Invoke();
    }

    public override void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!Controller.isPlayerTurn || _attackBuffer != null || !ctx.started) return;
        _attackBuffer = StartCoroutine(BufferAttack());
    }

    private IEnumerator BufferAttack()
    {
        yield return new WaitForSeconds(_inputBufferDuration);
        _animator.PlayAttack();
        Controller.onActionConfirmed?.Invoke();
        _attackBuffer = null;
    }

    public void OnAttackHit()
    {
        Vector2Int targetPos = _stats.CurTile.gridPosition + _lastMoveDir;

        if (_stats.curLevel.tiles.TryGetValue(targetPos, out Tile tile)
            && tile.CharacterStatsOnTile != null)
        {
            _stats.Attack(tile.CharacterStatsOnTile);
        }
    }

    public override void OnDash(InputAction.CallbackContext ctx)
    {
        if (!Controller.isPlayerTurn || _dashBuffer != null || !ctx.started) return;
        if (!canMove)
        {
            Shake();
            return;
        }
        _dashBuffer = StartCoroutine(BufferDash());
    }

    private IEnumerator BufferDash()
    {
        float time = 0f;
        Vector2 bufferedDir = Vector2.zero;

        while (time < _inputBufferDuration)
        {
            Vector2 inputDir = InputActions.PC.Move.ReadValue<Vector2>();
            if (inputDir != Vector2.zero)
                bufferedDir = inputDir;

            time += Time.deltaTime;
            yield return null;
        }

        StartDash(bufferedDir);
        _dashBuffer = null;
    }

    private void StartDash(Vector2 rawDir)
    {
        Vector2 dirVector = rawDir != Vector2.zero ? rawDir : new Vector2(_lastMoveDir.x, _lastMoveDir.y);
        Vector2Int dir = new(
            dirVector.x > 0 ? 1 : dirVector.x < 0 ? -1 : 0,
            dirVector.y > 0 ? 1 : dirVector.y < 0 ? -1 : 0
        );

        _lastMoveDir = dir;
        Vector2Int startPos = _stats.CurTile.gridPosition;
        int maxDistance = 0;

        for (int i = 1; i <= _dashDistance; i++)
        {
            Vector2Int nextPos = startPos + dir * i;

            if (!_stats.curLevel.tiles.TryGetValue(nextPos, out Tile tile) || !tile.IsWalkable || tile.CharacterStatsOnTile != null)
                break;

            maxDistance = i;
        }

        if (maxDistance <= 0) return;

        _dashStartHealth = _stats.CurrentHealth;
        _initialEnemies = new HashSet<EnemyStats>();

        foreach (var tile in _stats.tilesOnVision)
        {
            if (tile.CharacterStatsOnTile is EnemyStats enemy)
                _initialEnemies.Add(enemy);
        }

        TurnManager tm = TurnManager.Instance;
        _savedTurnSpeed = tm.turnSpeed;
        tm.turnSpeed *= 10;
        Controller.moveSpeed *= 10;
        Time.timeScale *= 10;

        _dashQueue = new Queue<Vector2Int>();
        for (int i = 0; i < maxDistance; i++)
            _dashQueue.Enqueue(dir);

        _dashCoroutine = StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        TurnManager tm = TurnManager.Instance;
        bool stop = false;
        bool waitForTurn = true;

        while (_dashQueue.Count > 0 && !stop)
        {
            if (!waitForTurn)
                yield return new WaitUntil(() => Controller.isPlayerTurn);
            waitForTurn = false;

            Vector2Int step = _dashQueue.Dequeue();
            Vector2Int cur = _stats.CurTile.gridPosition;
            Vector2Int next = cur + step;

            if (!_stats.curLevel.tiles.TryGetValue(next, out Tile tile)
                || !tile.IsWalkable
                || tile.CharacterStatsOnTile != null
                || _stats.CurrentHealth < _dashStartHealth)
                break;

            foreach (var visTile in _stats.tilesOnVision)
            {
                if (visTile.CharacterStatsOnTile is EnemyStats enemy && !_initialEnemies.Contains(enemy))
                {
                    stop = true;
                    break;
                }
            }

            if (step.x != 0)
                _spriteRenderer.flipX = step.x < 0;

            _animator.PlayMove();
            _stats.CurTile.CharacterStatsOnTile = null;
            _stats.CurTile = tile;
            tile.CharacterStatsOnTile = _stats;

            yield return transform.DOMove(new Vector3(next.x, next.y, 0), 0.01f).SetEase(Ease.Linear).WaitForCompletion();

            Controller.onActionConfirmed?.Invoke();
        }

        yield return new WaitUntil(() => Controller.isPlayerTurn);

        tm.turnSpeed = _savedTurnSpeed;
        Controller.moveSpeed /= 10;
        Time.timeScale /= 10;
        _dashCoroutine = null;
    }

    public override void OnCtrl(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _isCtrlHeld = true;
            _highlightCoroutine = StartCoroutine(HighlightLoop());
        }
        else if (ctx.canceled)
        {
            _isCtrlHeld = false;
            if (_highlightCoroutine != null)
                StopCoroutine(_highlightCoroutine);
            HideHighlight();
        }
    }

    private IEnumerator HighlightLoop()
    {
        if (_highlightInstance == null)
            _highlightInstance = Instantiate(highlightPrefab);
        else
            _highlightInstance.SetActive(true);

        while (_isCtrlHeld)
        {
            UpdateHighlightPosition();
            yield return null;
        }

        HideHighlight();
    }

    private void UpdateHighlightPosition()
    {
        Vector2Int cur = _stats.CurTile.gridPosition;
        Vector2Int tgt = cur + _lastMoveDir;

        if (!_stats.curLevel.tiles.TryGetValue(tgt, out Tile tile))
        {
            _highlightInstance.SetActive(false);
            return;
        }

        _highlightInstance.SetActive(true);
        _highlightInstance.transform.position = new Vector3(tgt.x, tgt.y, 0);

        SpriteRenderer sr = _highlightInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            int y = tgt.y;
            sr.sortingOrder = y == 0 ? -y - 1 : (y > 0 ? -y * 11 : -y * 9);
        }
    }

    private void HideHighlight()
    {
        if (_highlightInstance != null)
            _highlightInstance.SetActive(false);
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

    public override void OnInteract(InputAction.CallbackContext ctx) { }
    public override void OnCancel(InputAction.CallbackContext ctx) { }
    public override void OnMenu(InputAction.CallbackContext ctx) { }
    public override void OnAttackDirection(InputAction.CallbackContext ctx) { }
    public override void OnMousePosition(InputAction.CallbackContext ctx) { }
    public override void OnMouseClick(InputAction.CallbackContext ctx) { }
}