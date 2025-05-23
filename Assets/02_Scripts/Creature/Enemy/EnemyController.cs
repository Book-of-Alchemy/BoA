using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;

public enum EnemyState
{
    Idle,
    Chase,
    Attack
}

public class EnemyController : MonoBehaviour
{
    public float MoveSpeed => TurnManager.Instance.turnSpeed;
    public bool IsDead => _enemyStats == null ? true : _enemyStats.CurrentHealth <= 0;
    public bool canMove = true;
    public bool isConfused = false;
    public EnemyState _currentState = EnemyState.Idle;
    public EnemySkill curSkill;
    private Tile _lastCheckedTile;
    public Tile LastCheckedTile//serializable 클래스의 인스턴스는 public 또는 serialized 로 생성시 직렬화를 하기위해 유니티에서 new 인스턴스를 생성함 따라서 null로 처리하기위해 프로퍼티로 사용하여야함
    {
        get => _lastCheckedTile;
        set => _lastCheckedTile = value;
    }
    public event Action onTurnEnd;
    IdleBaseBehaviour idleBehaviour;
    ChaseBaseBehaviour chaseBehaviour;
    AttackBaseBehaviour attackBehaviour;
    IMovementStrategy moveStrategy;
    CharacterAnimator characterAnimator;
    CharacterStats _targetStats;
    EnemyStats _enemyStats;
    Tween _currentTween;
    SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _enemyStats = gameObject.GetComponent<EnemyStats>();
        idleBehaviour = GetComponent<IdleBaseBehaviour>();
        chaseBehaviour = GetComponent<ChaseBaseBehaviour>();
        attackBehaviour = GetComponent<AttackBaseBehaviour>();
        moveStrategy = GetComponent<IMovementStrategy>();
        characterAnimator = GetComponent<CharacterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _targetStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
        _spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y) * 10;
        UpdateVisual();
    }

    public void TakeTurn()
    {
        if (IsDead)
        {
            EndTurn();
            return;
        }

        if (_currentTween != null && _currentTween.IsActive())
            _currentTween.Kill();

        switch (_currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;
            case EnemyState.Chase:
                HandleChase();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
        }
    }
    public void ChangeState(EnemyState newState)
    {
        _currentState = newState;
        // 플레이어를 감지했을 때 추격 혹은 공격마다 뒤집기
        if (newState == EnemyState.Chase || newState == EnemyState.Attack)
            FlipTowardsPlayer();

        TakeTurn(); // 상태 바뀌자마자 바로 행동 즉 실질 행동시작 전에 상태체크를우선해야함
    }
    /// <summary>
    /// 각 behaviour 마다 actioncost 반환
    /// </summary>
    /// <returns></returns>
    public float GetCurrentActionCostMultiplier()
    {
        return _currentState switch
        {
            EnemyState.Idle => idleBehaviour?.ActionCostMultiPlier ?? 1f,
            EnemyState.Chase => chaseBehaviour?.ActionCostMultiPlier ?? 1f,
            EnemyState.Attack => attackBehaviour?.ActionCostMultiPlier ?? 1f,
            _ => 1f
        };
    }

    private void HandleIdle()
    {
        idleBehaviour?.Excute();
        FlipTowardsPlayer();
    }

    private void HandleChase()
    {
        chaseBehaviour?.Excute();
        FlipTowardsPlayer();
    }

    private void HandleAttack()
    {
        attackBehaviour?.Excute();
        FlipTowardsPlayer();
    }

    public void EndTurn()
    {
        onTurnEnd?.Invoke();
        UpdateVisual();
    }

    public void MoveTo(Vector2Int targetPosition, Action onComplete = null)
    {
        if (!canMove)
        {
            EndTurn();
            return;
        }

        if (isConfused)
        {
            List<Tile> tiles = TileUtility.GetAdjacentTileList(_enemyStats.curLevel, _enemyStats.CurTile, true);
            foreach (Tile tile in tiles.ToArray())
            {
                if (tile.IsWalkable)
                    tiles.Remove(tile);
            }

            if (tiles != null)
                targetPosition = tiles[UnityEngine.Random.Range(0, tiles.Count)].gridPosition;
        }

        Vector3 position = new Vector3(targetPosition.x, targetPosition.y, 0);
        _spriteRenderer.sortingOrder = -targetPosition.y * 10+1;
        if (moveStrategy != null)
            moveStrategy.Move(this.transform, position, characterAnimator, onComplete);
        else
            BasicMove(position, onComplete);
    }

    void BasicMove(Vector3 targetPosition, Action onComplete = null)
    {
        characterAnimator?.PlayMove();

        _currentTween?.Kill(); // 기존 움직임 취소
        _currentTween = transform.DOMove(targetPosition, 1f / MoveSpeed)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }
    //애니메이션 이벤트에 추가해야함
    public void OnAttackHit()
    {
        if (_targetStats != null)
            _enemyStats.Attack(_targetStats);
        EndTurn();
    }
    public void Attack(Action onComplete = null)
    {
        onComplete?.Invoke();
        characterAnimator?.PlayAttack();
    }

    public void SetCurrentSkill(EnemySkill skill)
    {
        curSkill = skill;
    }

    public void Skill_0(Action onComplete = null)
    {
        onComplete?.Invoke();
        characterAnimator?.PlaySkill_0();
    }

    public void Skill_1(Action onComplete = null)
    {
        onComplete?.Invoke();
        characterAnimator?.PlaySkill_1();
    }
    public void OnSkillHit()
    {
        if (curSkill != null)
            curSkill.Use();
    }



    // 플레이어 바라보기 메서드(감지되면 호출하기)
    private void FlipTowardsPlayer()
    {
        if (_targetStats == null) return;  // 안전장치

        float dirX = _targetStats.transform.position.x - transform.position.x;//플레이어 방향 구하기
        if (Mathf.Abs(dirX) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dirX);
            transform.localScale = scale;
        }
    }

    public void UpdateVisual()
    {
        if (!_spriteRenderer || !_enemyStats)
            return;

        if (_enemyStats.CurTile == null)
            return;


        bool shouldEnable;

        if (_enemyStats.CurTile.IsOnSight)
        {
            shouldEnable = true;
        }
        else
        {
            shouldEnable = false;
        }

        _spriteRenderer.enabled = shouldEnable;
    }
}
