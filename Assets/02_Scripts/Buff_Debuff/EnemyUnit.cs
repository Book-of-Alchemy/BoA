using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyUnit : UnitBase
{
    EnemyController _controller;

    protected override void Awake()
    {
        base.Awake();
        _controller = GetComponent<EnemyController>();
        _controller.onTurnEnd += OnTurnEnd;
    }
    private void OnEnable()
    {
        if (_controller == null)
            return;
        _controller.onTurnEnd -= OnTurnEnd;
        _controller.onTurnEnd += OnTurnEnd;
    }
    private void OnDisable()
    {
        if (_controller == null)
            return;
        _controller.onTurnEnd -= OnTurnEnd;
    }

    public void UpdateVisual()
    {
        _controller.UpdateVisual();
    }

    public override void PerformAction()
    {
        _controller.TakeTurn();
    }

    public override int GetModifiedActionCost()
    {
        return Mathf.RoundToInt(ActionCost * _controller.GetCurrentActionCostMultiplier());
    }
}
