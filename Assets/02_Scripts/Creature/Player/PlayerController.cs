using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float MoveActionCost = 1f;
    public LayerMask UnitLayer;
    public LayerMask ObstacleLayer;
    public float InputBufferDuration = 0.1f;

    private PlayerStats _playerStats;
    private PlayerInputActions _inputActions;
    private CharacterAnimator _animator;
    private bool _isMoving;
    private Coroutine _moveBufferCoroutine;
    private Coroutine _attackBufferCoroutine;
    private Vector2 _lastMoveDirection = Vector2.down;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _animator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _inputActions = new PlayerInputActions();

        _inputActions.PC.Move.started += ctx =>
        {
            if (!_isMoving
                && _playerStats.BuffManager.GetFinalActionPoints() >= MoveActionCost
                && _moveBufferCoroutine == null)
            {
                _moveBufferCoroutine = StartCoroutine(BufferAndMove());
            }
        };

        _inputActions.PC.Attack.started += ctx =>
        {
            if (_attackBufferCoroutine == null)
            {
                _attackBufferCoroutine = StartCoroutine(BufferAndAttack());
            }
        };
    }

    private void OnEnable() => _inputActions.PC.Enable();
    private void OnDisable() => _inputActions.PC.Disable();

    private IEnumerator BufferAndMove()
    {
        float elapsed = 0f;
        Vector2 bufferedInput = Vector2.zero;

        while (elapsed < InputBufferDuration)
        {
            Vector2 current = _inputActions.PC.Move.ReadValue<Vector2>();
            if (current != Vector2.zero) bufferedInput = current;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (bufferedInput != Vector2.zero)
        {
            ExecuteMove(bufferedInput);
        }

        _moveBufferCoroutine = null;
    }

    private IEnumerator BufferAndAttack()
    {
        float elapsed = 0f;
        Vector2 bufferedInput = Vector2.zero;

        while (elapsed < InputBufferDuration)
        {
            Vector2 current = _inputActions.PC.Move.ReadValue<Vector2>();
            if (current != Vector2.zero) bufferedInput = current;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector2 rawInput = bufferedInput != Vector2.zero ? bufferedInput : _lastMoveDirection;
        Vector2Int offset = new Vector2Int(
            rawInput.x > 0 ? 1 : rawInput.x < 0 ? -1 : 0,
            rawInput.y > 0 ? 1 : rawInput.y < 0 ? -1 : 0
        );
        _lastMoveDirection = offset;
        DoAttack(offset);
        _attackBufferCoroutine = null;
    }

    private void ExecuteMove(Vector2 rawInput)
    {
        Vector2Int curCell = _playerStats.curTile.gridPosition;
        Vector2Int offset = new Vector2Int(
            rawInput.x > 0 ? 1 : rawInput.x < 0 ? -1 : 0,
            rawInput.y > 0 ? 1 : rawInput.y < 0 ? -1 : 0
        );
        
        if (offset.x != 0)
            _spriteRenderer.flipX = offset.x < 0;

        _lastMoveDirection = offset;
        Vector2Int tgtCell = curCell + offset;

        if (!_playerStats.curLevel.tiles.TryGetValue(tgtCell, out var tile)
            || tile.CharacterStatsOnTile != null
            || !tile.IsWalkable)
            return;

        _isMoving = true;
        _playerStats.curTile.CharacterStatsOnTile = null;
        _playerStats.curTile = tile;
        tile.CharacterStatsOnTile = _playerStats;

        Vector3 dest = new Vector3(tgtCell.x, tgtCell.y, 0f);
        float duration = Vector3.Distance(transform.position, dest) / MoveSpeed;

        _animator.PlayMove();
        transform
            .DOMove(dest, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isMoving = false;
                _playerStats.BuffManager.ApplyBuff(-MoveActionCost, 0);
            });
    }

    private void DoAttack(Vector2 dir)
    {
        if (dir.x != 0)
            _spriteRenderer.flipX = dir.x < 0;

        _animator.PlayAttack();
        PerformAttackRaycast(dir);
        _playerStats.BuffManager.ApplyBuff(-MoveActionCost, 0);
    }

    private void PerformAttackRaycast(Vector2 dir)
    {
        var col = GetComponent<Collider2D>();
        float originOffset = col.bounds.extents.magnitude + 0.1f;
        Vector2 origin = (Vector2)transform.position + dir.normalized * originOffset;
        Vector2 direction = dir.normalized;
        Debug.DrawRay(origin, direction * 0.5f, Color.red, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 0.5f, UnitLayer);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                _playerStats.Attack(enemyStats);
            }
        }
    }
}
