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

public class EnemyController2 : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float DetectionRange = 10f;
    public EnemyState _currentState = EnemyState.Idle;
    public Tile lastCheckedTile = null;
    IdleBaseBehaviour idleBehaviour;
    ChaseBaseBehaviour chaseBehaviour;
    AttackBaseBehaviour attackBehaviour;
    IMovementStrategy moveStrategy;
    EnemyAnimatorController animationController;
    PlayerStats _playerStats;
    EnemyStats _enemyStats;
    Tween _currentTween;

    void Awake()
    {
        GameManager.Instance.RegisterEnemy(GetComponent<EnemyStats>());
        _enemyStats = gameObject.GetComponent<EnemyStats>();
        idleBehaviour = GetComponent<IdleBaseBehaviour>();
        chaseBehaviour = GetComponent<ChaseBaseBehaviour>();
        attackBehaviour = GetComponent<AttackBaseBehaviour>();
        moveStrategy = GetComponent<IMovementStrategy>();
        animationController = GetComponent<EnemyAnimatorController>();
    }

    void Start()
    {
        _playerStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
    }

    public IEnumerator TakeTurn()
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

        yield break;
    }

    public void ChangeState(EnemyState newState)
    {
        _currentState = newState;
        TakeTurn(); // 상태 바뀌자마자 바로 행동 즉 실질 행동시작 전에 상태체크를우선해야함
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

        if(moveStrategy != null)
            moveStrategy.Move(this.transform , position, animationController, onComplete);
        else
            BasicMove(position, onComplete);
    }

    void BasicMove(Vector3 targetPosition,  Action onComplete = null)
    {
        animationController?.TriggerMove();

        _currentTween?.Kill(); // 기존 움직임 취소
        _currentTween = transform.DOMove(targetPosition, 1f / moveSpeed)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                animationController?.TriggerIdle();
                onComplete?.Invoke();
                });
    }

    public void Attack( Action onComplete = null)
    {
        animationController?.TriggerAttack();

        onComplete?.Invoke();
    }
    /*private void HandleAttack()
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
    }*/

}
