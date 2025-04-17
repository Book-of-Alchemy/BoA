using UnityEngine;
using DG.Tweening;

public enum EnemyState
{
    Idle,
    Chase,
    Attack
}

public class EnemyController2 : MonoBehaviour
{
    public float MoveSpeed = 3f;
    public float DetectionRange = 10f;
    public EnemyState _currentState = EnemyState.Idle;
    public EnemyController2 _controller;
    public Tile lastCheckedTile = null;
    IdleBaseBehaviour idleBehaviour;
    ChaseBaseBehaviour chaseBehaviour;
    AttackBaseBehaviour attackBehaviour;
    PlayerStats _playerStats;
    Tween _currentTween;

    void Awake()
    {
        GameManager.Instance.RegisterEnemy(GetComponent<EnemyStats>());
        _controller = GetComponent<EnemyController2>();
        idleBehaviour = GetComponent<IdleBaseBehaviour>();
        chaseBehaviour = GetComponent<ChaseBaseBehaviour>();
        attackBehaviour = GetComponent<AttackBaseBehaviour>();

    }

    void Start()
    {
        _playerStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
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
                break;
        }
    }

    public void ChangeState(EnemyState newState)
    {
        _currentState = newState;
        TakeTurn(); // 상태 바뀌자마자 바로 행동
    }

    private void HandleIdle()
    {
        float dist = GetDistanceToPlayer();
        if (dist <= 1.0f)
            _currentState = EnemyState.Attack;
        else if (dist <= DetectionRange)
            _currentState = EnemyState.Chase;

        _currentTween = DOVirtual.DelayedCall(0.5f, TakeTurn);
    }

    private void HandleChase()
    {
        float dist = GetDistanceToPlayer();

        if (dist <= 1.0f)
        {
            _currentState = EnemyState.Attack;
            TakeTurn(); // 즉시 다음 상태 처리
            return;
        }

        if (dist > DetectionRange)
        {
            _currentState = EnemyState.Idle;
            TakeTurn();
            return;
        }

        Vector3 dir = (_playerStats.transform.position - transform.position);
        float dirX = Mathf.Abs(dir.x) > 0.1f ? Mathf.Sign(dir.x) : 0f;
        float dirY = Mathf.Abs(dir.y) > 0.1f ? Mathf.Sign(dir.y) : 0f;
        Vector3 currentCell = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), 0);
        Vector3 targetCell = currentCell + new Vector3(dirX, dirY, 0);
        Vector3 targetPos = targetCell + new Vector3(0.5f, 0.5f, 0);

        Collider2D hit = Physics2D.OverlapPoint(targetPos);
        if (hit != null && hit.CompareTag("Player"))
        {
            _currentState = EnemyState.Attack;
            TakeTurn();
            return;
        }

        float duration = 1f / MoveSpeed;
        _currentTween = transform.DOMove(targetPos, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _currentState = EnemyState.Idle;
                TakeTurn();
            });
    }

    private void HandleAttack()
    {
        float dist = GetDistanceToPlayer();
        if (dist > 1.0f)
        {
            _currentState = dist <= DetectionRange ? EnemyState.Chase : EnemyState.Idle;
            TakeTurn();
            return;
        }

        Debug.Log($"{gameObject.name}이(가) 플레이어를 공격합니다.");
        GetComponent<EnemyStats>().Attack(_playerStats);

        _currentTween = DOVirtual.DelayedCall(0.5f, () =>
        {
            _currentState = EnemyState.Idle;
            TakeTurn();
        });
    }

    private float GetDistanceToPlayer()
    {
        Vector2 enemyPos = new Vector2(Mathf.Floor(transform.position.x) + 0.5f,
                                       Mathf.Floor(transform.position.y) + 0.5f);
        Vector2 playerPos = new Vector2(Mathf.Floor(_playerStats.transform.position.x) + 0.5f,
                                        Mathf.Floor(_playerStats.transform.position.y) + 0.5f);
        return Vector2.Distance(enemyPos, playerPos);
    }
}
