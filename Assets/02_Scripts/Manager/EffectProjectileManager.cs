using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectProjectileManager : Singleton<EffectProjectileManager>
{
    public EffectPool effectPool;
    public ProjectilePool projectilePool;
    public Vector3 offset = new Vector3(0, 0.5f, 0);

    public void PlayEffect(Vector2Int target, int id, bool isFlip = false)
    {
        Effect effect = effectPool.GetFromPool(id, target);
        effect.spriteRenderer.flipX = isFlip;
        effect.Play();
        DOVirtual.DelayedCall
            (
            effect.animationLength,
            () => effectPool.ReturnToPool(effect)
            );
    }
    public void PlayEffect(Vector2Int origin, Vector2Int target, int id)
    {
        bool isFlip = target.x - origin.x < 0 ? true : false;
        Effect effect = effectPool.GetFromPool(id, target);
        effect.spriteRenderer.flipX = isFlip;
        effect.Play();
        DOVirtual.DelayedCall
            (
            effect.animationLength,
            () => effectPool.ReturnToPool(effect)
            );
    }
    public void LaunchProjectile(Vector2Int origin, Vector2Int target, int id, Action onComplete = null)
    {
        Projectile projectile = projectilePool.GetFromPool(id, origin, this.offset);
        bool isFlip = target.x - origin.x < 0 ? true : false;
        projectile.spriteRenderer.flipX = isFlip;
        Vector3 adjustedPosition = new Vector3(target.x, target.y, 0) + offset;
        projectile.Play();
        projectile.transform.DOMove(adjustedPosition, 0.1f)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
                projectilePool.ReturnToPool(projectile);
            });
    }

    public void LaunchProjectile(Vector2Int origin, Vector2Int target, int projectileId, int effectId, Action onComplete = null)
    {
        Projectile projectile = projectilePool.GetFromPool(projectileId, origin, this.offset);
        bool isFlip = target.x - origin.x < 0 ? true : false;
        projectile.spriteRenderer.flipX = isFlip;
        Vector3 adjustedPosition = new Vector3(target.x, target.y, 0) + offset;
        projectile.Play();
        projectile.transform.DOMove(adjustedPosition, 0.1f)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
                projectilePool.ReturnToPool(projectile);
                PlayEffect(target, effectId, isFlip);
            });
    }
}
