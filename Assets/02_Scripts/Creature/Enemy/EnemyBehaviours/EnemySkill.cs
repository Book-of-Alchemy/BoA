using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemySkill : MonoBehaviour
{
    public int lastUseTime = -9999;
    public abstract int cooldown { get; }
    AttackBaseBehaviour attackBaseBehaviour;
    protected virtual void Awake()
    {
        attackBaseBehaviour = GetComponent<AttackBaseBehaviour>();
    }

    public abstract bool CanUse();
    public abstract void Use(); //enemystat을 참조하여 데미지를 주는 로직을 담고있어야함
}
