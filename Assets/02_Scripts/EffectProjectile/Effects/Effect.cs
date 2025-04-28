using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Effect : MonoBehaviour, IPoolableId
{
    [SerializeField]
    private int id;
    public int Id { get => id; set => id = value; }
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public float animationLength;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animationLength = animator.runtimeAnimatorController.animationClips[0].length;
    }

    public void Play()
    {
        animator.Play(0);
    }
}
