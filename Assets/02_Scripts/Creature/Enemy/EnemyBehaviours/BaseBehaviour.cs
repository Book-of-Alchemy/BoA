using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviour
{
    public void Excute();
}

public abstract class BaseBehaviour : MonoBehaviour, IBehaviour
{
    protected EnemyController2 controller;
    protected EnemyStats enemyStats;

    protected virtual void Awake()
    {
        controller = GetComponent<EnemyController2>();
        enemyStats = GetComponent<EnemyStats>();
    }

    public abstract void Excute();
}
