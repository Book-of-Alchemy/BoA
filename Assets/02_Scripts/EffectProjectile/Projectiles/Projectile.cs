using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolableId
{
    [SerializeField]
    private int id;
    public int Id { get => id; set => id = value; }
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Play()
    {
        if (animator != null && animator.runtimeAnimatorController != null && animator.runtimeAnimatorController.animationClips.Length > 0)
        {
            animator.Play(0);
        }
    }
}
