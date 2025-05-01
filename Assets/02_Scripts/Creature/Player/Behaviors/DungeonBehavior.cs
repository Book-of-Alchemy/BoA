using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(PlayerStats), typeof(CharacterAnimator), typeof(SpriteRenderer))]
public class DungeonBehavior : PlayerBaseBehavior
{
    [Header("Dungeon Settings")]
    [SerializeField] private float inputBufferDuration = 0.1f;
    [SerializeField] private int dashDistance = 3;
    public GameObject highlightPrefab;

    private PlayerStats stats;
    private CharacterAnimator animator;
    private SpriteRenderer spriteRenderer;
    private bool isMoving;
    private bool isCtrlHeld;
    private Coroutine moveBuffer, attackBuffer, dashBuffer, dashCoroutine, highlightCoroutine;
    private Vector2Int lastMoveDir = Vector2Int.right;
    private Queue<Vector2Int> dashQueue;
    private float savedTurnSpeed, dashStartHealth;
    private HashSet<EnemyStats> initialEnemies;
    private GameObject highlightInstance;
    private BaseItem currentItem;

    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);
        stats = GetComponent<PlayerStats>();
        animator = GetComponent<CharacterAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnMove(InputAction.CallbackContext ctx)
    {
        if (!Controller.isPlayerTurn || isMoving || moveBuffer != null || !ctx.started) return;
        moveBuffer = StartCoroutine(BufferMove());
    }
    IEnumerator BufferMove()
    {
        float t = 0; Vector2 buf = Vector2.zero;
        while (t < inputBufferDuration) { if (InputActions.PC.Move.ReadValue<Vector2>() != Vector2.zero) buf = InputActions.PC.Move.ReadValue<Vector2>(); t += Time.deltaTime; yield return null; }
        ExecuteMove(buf); moveBuffer = null;
    }
    void ExecuteMove(Vector2 raw)
    {
        if (raw == Vector2.zero) return;
        var o = new Vector2Int(raw.x > 0 ? 1 : raw.x < 0 ? -1 : 0, raw.y > 0 ? 1 : raw.y < 0 ? -1 : 0);
        if (o != Vector2Int.zero) { lastMoveDir = o; spriteRenderer.flipX = o.x < 0; }
        if (isCtrlHeld) return;
        var cur = stats.CurTile.gridPosition; var tgt = cur + o;
        if (!stats.curLevel.tiles.TryGetValue(tgt, out var tile) || !tile.IsWalkable || tile.CharacterStatsOnTile != null) return;
        isMoving = true; stats.CurTile.CharacterStatsOnTile = null; stats.CurTile = tile; tile.CharacterStatsOnTile = stats;
        spriteRenderer.sortingOrder = -tgt.y * 10 + 1; animator.PlayMove();
        transform.DOMove(new Vector3(tgt.x, tgt.y, 0), 0.1f).SetEase(Ease.Linear).OnComplete(() => isMoving = false);
        Controller.onActionConfirmed?.Invoke();
    }

    public override void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!Controller.isPlayerTurn || attackBuffer != null || !ctx.started) return;
        attackBuffer = StartCoroutine(BufferAttack());
    }
    IEnumerator BufferAttack() { yield return new WaitForSeconds(inputBufferDuration); animator.PlayAttack(); Controller.onActionConfirmed?.Invoke(); attackBuffer = null; }
    public void OnAttackHit()
    {
        var dir = lastMoveDir; var tp = stats.CurTile.gridPosition + dir;
        if (stats.curLevel.tiles.TryGetValue(tp, out var tile) && tile.CharacterStatsOnTile != null) stats.Attack(tile.CharacterStatsOnTile);
    }

    public override void OnDash(InputAction.CallbackContext ctx)
    {
        if (!Controller.isPlayerTurn || dashBuffer != null || !ctx.started) return;
        dashBuffer = StartCoroutine(BufferDash());
    }
    IEnumerator BufferDash() { float t = 0; Vector2 buf = Vector2.zero; while (t < inputBufferDuration) { var v = InputActions.PC.Move.ReadValue<Vector2>(); if (v != Vector2.zero) buf = v; t += Time.deltaTime; yield return null; } StartDash(buf); dashBuffer = null; }
    void StartDash(Vector2 raw)
    {
        var dr = raw != Vector2.zero ? raw : new Vector2(lastMoveDir.x, lastMoveDir.y);
        var o = new Vector2Int(dr.x > 0 ? 1 : dr.x < 0 ? -1 : 0, dr.y > 0 ? 1 : dr.y < 0 ? -1 : 0); lastMoveDir = o;
        var start = stats.CurTile.gridPosition; int max = 0; for (int i = 1; i <= dashDistance; i++) { var n = start + o * i; if (!stats.curLevel.tiles.TryGetValue(n, out var t) || !t.IsWalkable || t.CharacterStatsOnTile != null) break; max = i; }
        if (max <= 0) return;
        dashStartHealth = stats.CurrentHealth; initialEnemies = new HashSet<EnemyStats>(); foreach (var v in stats.tilesOnVision) if (v.CharacterStatsOnTile is EnemyStats e) initialEnemies.Add(e);
        var tm = TurnManager.Instance; savedTurnSpeed = tm.turnSpeed; tm.turnSpeed *= 10; Controller.moveSpeed *= 10; Time.timeScale *= 10;
        dashQueue = new Queue<Vector2Int>(); for (int i = 0; i < max; i++) dashQueue.Enqueue(o);
        dashCoroutine = StartCoroutine(DashCoroutine());
    }
    IEnumerator DashCoroutine()
    {
        var tm = TurnManager.Instance; bool c = false; bool f = true;
        while (dashQueue.Count > 0 && !c)
        {
            if (!f) yield return new WaitUntil(() => Controller.isPlayerTurn); f = false;
            var s = dashQueue.Dequeue(); var cur = stats.CurTile.gridPosition; var nxt = cur + s;
            if (!stats.curLevel.tiles.TryGetValue(nxt, out var tile) || !tile.IsWalkable || tile.CharacterStatsOnTile != null || stats.CurrentHealth < dashStartHealth) break;
            foreach (var v in stats.tilesOnVision) if (v.CharacterStatsOnTile is EnemyStats e && !initialEnemies.Contains(e)) { c = true; break; }
            spriteRenderer.flipX = s.x < 0; animator.PlayMove(); stats.CurTile.CharacterStatsOnTile = null; stats.CurTile = tile; tile.CharacterStatsOnTile = stats;
            yield return transform.DOMove(new Vector3(nxt.x, nxt.y, 0), 0.01f).SetEase(Ease.Linear).WaitForCompletion();
            Controller.onActionConfirmed?.Invoke();
            foreach (var v in stats.tilesOnVision) if (v.CharacterStatsOnTile is EnemyStats e && !initialEnemies.Contains(e)) { c = true; break; }
        }
        yield return new WaitUntil(() => Controller.isPlayerTurn);
        tm.turnSpeed = savedTurnSpeed; Controller.moveSpeed /= 10; Time.timeScale /= 10; dashCoroutine = null;
    }

    public override void OnCtrl(InputAction.CallbackContext ctx)
    {
        if (ctx.started) { isCtrlHeld = true; highlightCoroutine = StartCoroutine(HighlightLoop()); }
        else if (ctx.canceled) { isCtrlHeld = false; if (highlightCoroutine != null) StopCoroutine(highlightCoroutine); HideHighlight(); }
    }
    IEnumerator HighlightLoop()
    {
        if (highlightInstance == null) highlightInstance = Instantiate(highlightPrefab); else highlightInstance.SetActive(true);
        while (isCtrlHeld) { UpdateHighlightPosition(); yield return null; }
        HideHighlight();
    }
    void UpdateHighlightPosition() { var cur = stats.CurTile.gridPosition; var tgt = cur + lastMoveDir; if (!stats.curLevel.tiles.TryGetValue(tgt, out var t)) { highlightInstance.SetActive(false); return; } highlightInstance.SetActive(true); highlightInstance.transform.position = new Vector3(tgt.x, tgt.y, 0); var sr = highlightInstance.GetComponent<SpriteRenderer>(); sr.sortingOrder = tgt.y >= 0 ? -tgt.y * 11 : -tgt.y * 9; }
    void HideHighlight() { if (highlightInstance != null) highlightInstance.SetActive(false); }

    // 아이템 사용
    public void UseItem(ItemData data)
    {
        currentItem = Instantiate(data.itemPrefab).GetComponent<BaseItem>();
        if (currentItem == null) return;
        currentItem.ItemUseDone += HandleItemUseDone;
        currentItem.UseItem(data);
    }
    void HandleItemUseDone() { if (currentItem != null) { currentItem.ItemUseDone -= HandleItemUseDone; Controller.onActionConfirmed?.Invoke(); currentItem = null; } }
    public override void OnInteract(InputAction.CallbackContext ctx) {  }
    public override void OnCancel(InputAction.CallbackContext ctx) { }
    public override void OnMenu(InputAction.CallbackContext ctx) { }
    public override void OnAttackDirection(InputAction.CallbackContext ctx) { }
    public override void OnMousePosition(InputAction.CallbackContext ctx) { }
    public override void OnMouseClick(InputAction.CallbackContext ctx) { }

}
