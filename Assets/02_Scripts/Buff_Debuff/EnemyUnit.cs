using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    EnemyController _controller;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }
    public override void PerformAction()
    {
        _controller.TakeTurn();
    }
}
