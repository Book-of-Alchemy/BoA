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
        if (animator != null && animator.runtimeAnimatorController != null && animator.runtimeAnimatorController.animationClips.Length > 0)
        {
            animator.Play(0);
        }
    }
}
