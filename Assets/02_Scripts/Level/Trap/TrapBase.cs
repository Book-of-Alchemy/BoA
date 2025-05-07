using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class TrapBase : MonoBehaviour
{
    public Tile tile;
    public TrapData trapData;
    public List<Tile> effectTiles;
    [SerializeField]
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
        StartCoroutine(DelayedInit());//임시코드
    }

    private IEnumerator DelayedInit()
    {
        yield return null; // 한 프레임 대기
        
        tile = GameManager.Instance.PlayerTransform.curLevel.tiles[new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y))];
        tile.TrpaOnTile = this;
    }

    public virtual void Initialize(Tile tile)
    {
        this.tile = tile;
        tile.onCharacterChanged -= Execute;
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
        //함정에 유효한 타입인지 확인과정 추가 ex 비행타입

        if (!IsDetected)
            IsDetected = true;
        TriggerActivate();
        Action();
        StartCoroutine(DestroyAfterDelay(2f)); 
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        tile.onCharacterChanged -= Execute;
        yield return new WaitForSeconds(delay);
        tile.TrpaOnTile = null;
        Destroy(gameObject);
    }

    void TriggerActivate()
    {
        animator.SetTrigger("Activate");
    }

    public abstract void Action();
}
