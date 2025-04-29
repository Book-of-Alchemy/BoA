using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : ObjectByIdPool<Projectile,ProjectileData>
{
    protected override void Awake()
    {
        Init();
        base.Awake();
    }

    void Init()
    {
        prefabWithIds = SODataManager.Instance.projectileDataBase.projectileDatas;
        EffectProjectileManager.Instance.projectilePool = this;
    }
}
