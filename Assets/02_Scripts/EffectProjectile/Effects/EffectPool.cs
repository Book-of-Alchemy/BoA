using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : ObjectByIdPool<Effect,EffectData>
{
    protected override void Awake()
    {
        Init();
        base.Awake();
    }

    void Init()
    {
        prefabWithIds = SODataManager.Instance.effectDataBase.effectDatas;
        EffectProjectileManager.Instance.effectPool = this;
    }
}
