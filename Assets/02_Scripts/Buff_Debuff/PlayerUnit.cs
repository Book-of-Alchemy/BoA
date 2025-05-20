using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase
{
    PlayerController controller;
    private bool isActionDone = false;
    public bool IsWaitingForInput => !isActionDone;
    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<PlayerController>();
        
    }

    private void OnEnable()
    {
        if (controller == null)
            return;
        controller.onActionConfirmed += OnActionConfirmed;
        //controller.onActionPerformed += OnTurnEnd;
        controller.onActionConfirmed += OnTurnEnd;
    }
    private void OnDisable()
    {
        controller.onActionConfirmed -= OnActionConfirmed;
        //controller.onActionPerformed -= OnTurnEnd;
        controller.onActionConfirmed -= OnTurnEnd;
    }

    public override void PerformAction()
    {
        controller.isPlayerTurn = true;
        isActionDone = false;
        
        Debug.Log("플레이어의 입력을 기다립니다.");
    }

    private void OnActionConfirmed()
    {
        isActionDone = true;
        controller.isPlayerTurn = false;
    }

}