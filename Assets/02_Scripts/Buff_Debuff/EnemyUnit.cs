using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    EnemyController _controller;

    protected override void Awake()
    {
        base.Awake();
        _controller = GetComponent<EnemyController>();
    }
    public override void PerformAction()
    {
        _controller.TakeTurn();
    }

    public override int GetModifiedActionCost()
    {
        return _controller.GetCurrentActionCost();
    }
}
