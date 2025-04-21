using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrapBase : MonoBehaviour
{
    public Tile tile;
    public TrapData trapData;
    public List<Tile> effectTiles;
    protected bool isDetected;
    public bool IsDetected
    {
        get => isDetected;
        set
        {
            isDetected = value;
            UpdateVisibility();
        }
    }
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    protected void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
            animator = GetComponent<Animator>();
        UpdateVisibility();
    }

    protected virtual void OnEnable()
    {
        if (tile != null)
            tile.onCharacterChanged += Execute;
    }

    protected virtual void OnDisable()
    {
        if (tile != null)
            tile.onCharacterChanged -= Execute;
    }

    public void UpdateVisibility()
    {
        spriteRenderer.enabled = IsDetected;
    }
    public virtual void Execute()
    {
        if (tile.CharacterStatsOnTile == null)
            return;
        //함정에 유효한 타입인지 확인과정

        if (!IsDetected) 
            IsDetected = true;
        TriggerActivate();
        Action();
    }

    void TriggerActivate()
    {
        animator.SetTrigger("Activate");
    }

    public abstract void Action();
}
