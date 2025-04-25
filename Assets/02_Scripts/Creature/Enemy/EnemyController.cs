using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;

public enum EnemyState
{
    Idle,
    Chase,
    Attack
}

public class EnemyController : MonoBehaviour
{
    public float MoveSpeed => TurnManager.Instance.turnSpeed;
    public float DetectionRange = 10f;
    public EnemyState _currentState = EnemyState.Idle;
    private Tile _lastCheckedTile;
    public Tile LastCheckedTile//serializable 클래스의 인스턴스는 public 또는 serialized 로 생성시 직렬화를 하기위해 유니티에서 new 인스턴스를 생성함 따라서 null로 처리하기위해 프로퍼티로 사용하여야함
    {
        get => _lastCheckedTile;
        set => _lastCheckedTile = value;
    }
    IdleBaseBehaviour idleBehaviour;
    ChaseBaseBehaviour chaseBehaviour;
    AttackBaseBehaviour attackBehaviour;
    IMovementStrategy moveStrategy;
    CharacterAnimator characterAnimator;
    PlayerStats _playerStats;
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
        _playerStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
        _spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y)*10;
    }

    public void TakeTurn()
    {
        if (_currentTween != null && _currentTween.IsActive()) _currentTween.Kill();

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
                FlipTowardsPlayer();
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
    public int GetCurrentActionCost()
    {
        return _currentState switch
        {
            EnemyState.Idle => idleBehaviour?.ActionCost ?? 10,
            EnemyState.Chase => chaseBehaviour?.ActionCost ?? 10,
            EnemyState.Attack => attackBehaviour?.ActionCost ?? 10,
            _ => 10
        };
    }

    private void HandleIdle()
    {
        idleBehaviour?.Excute();
    }

    private void HandleChase()
    {
        chaseBehaviour?.Excute();
    }

    private void HandleAttack()
    {
        attackBehaviour?.Excute();
    }


    public void MoveTo(Vector2Int targetPosition,  Action onComplete = null)
    {
        Vector3 position = new Vector3(targetPosition.x, targetPosition.y, 0);
        _spriteRenderer.sortingOrder = -targetPosition.y * 10;
        if (moveStrategy != null)
            moveStrategy.Move(this.transform , position, characterAnimator, onComplete);
        else
            BasicMove(position, onComplete);
    }

    void BasicMove(Vector3 targetPosition,  Action onComplete = null)
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
        if (_playerStats != null)
            _enemyStats.Attack(_playerStats);
    }
    public void Attack( Action onComplete = null)
    {
        onComplete?.Invoke();
        characterAnimator?.PlayAttack();
    }
    // 플레이어 바라보기 메서드(감지되면 호출하기)
    private void FlipTowardsPlayer()
    {
        if (_playerStats == null) return;  // 안전장치

        float dirX = _playerStats.transform.position.x - transform.position.x;//플레이어 방향 구하기
        if (Mathf.Abs(dirX) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dirX);
            transform.localScale = scale;
        }
    }


}
