using DG.Tweening;
using System;
using UnityEngine;

public interface IMovementStrategy
{
    void Move(Transform enemy, Vector3 targetPos, CharacterAnimator animationController = null, Action onComplete = null);
}

public class BasicMovement : IMovementStrategy
{
    public void Move(Transform enemy, Vector3 target, CharacterAnimator animationController = null, Action onComplete = null)
    {
        animationController?.SetMoving(true);

        enemy.DOMove(target, 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                animationController?.SetMoving(false);
                onComplete?.Invoke();
            });
    }
}