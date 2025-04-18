using DG.Tweening;
using System;
using UnityEngine;

public interface IMovementStrategy
{
    void Move(Transform enemy, Vector3 targetPos, EnemyAnimatorController animationController = null, Action onComplete = null);
}

public class BasicMovement : IMovementStrategy
{
    public void Move(Transform enemy, Vector3 target, EnemyAnimatorController animationController = null, Action onComplete = null)
    {
        animationController?.TriggerMove();

        enemy.DOMove(target, 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                animationController?.TriggerIdle();
                onComplete?.Invoke();
            });
    }
}