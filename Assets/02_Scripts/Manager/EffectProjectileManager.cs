using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectProjectileManager : Singleton<EffectProjectileManager>
{
    public EffectPool effectPool;
    public ProjectilePool projectilePool;
    public Vector3 offset = new Vector3(0,0.5f,0);

    public void PlayEffect(Vector2Int position, int id)
    {
        Effect effect = effectPool.GetFromPool(id, position);
        effect.Play();
        DOVirtual.DelayedCall
            (
            effect.animationLength,
            () => effectPool.ReturnToPool(effect)
            );
    }

    public void LaunchProjectile(Vector2Int origin, Vector2Int target, int id,Action onComplete = null)
    {
        Projectile projectile = projectilePool.GetFromPool(id, origin, this.offset);
        projectile.Play();
        projectile.transform.DOMove(new Vector3(target.x, target.y, 0), 0.1f)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
                projectilePool.ReturnToPool(projectile);
            });
    }

    public void LaunchProjectile(Vector2Int origin, Vector2Int target, int projectileId, int effectId, Action onComplete = null)
    {
        Projectile projectile = projectilePool.GetFromPool(projectileId, origin, this.offset);
        projectile.Play();
        projectile.transform.DOMove(new Vector3(target.x, target.y, 0), 0.1f)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
                projectilePool.ReturnToPool(projectile);
                PlayEffect(target, effectId);
            });
    }
}
