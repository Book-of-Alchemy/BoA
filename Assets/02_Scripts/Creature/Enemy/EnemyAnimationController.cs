using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerMove()
    {
        animator?.SetTrigger("Move");
    }

    public void TriggerIdle()
    {
        animator?.SetTrigger("Idle");
    }

    public void TriggerAttack()
    {
        animator?.SetTrigger("Attack");
    }

    public void TriggerHit()
    {
        animator?.SetTrigger("Hit");
    }
}

