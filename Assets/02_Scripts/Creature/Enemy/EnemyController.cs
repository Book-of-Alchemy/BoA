using UnityEngine;
using System.Collections;
using DG.Tweening;
public enum EnemyAIType
{
    BasicTracking,
    // 기타 AI 타입이 있을 수 있음
}

public class EnemyController : MonoBehaviour
{
    public EnemyAIType AIType = EnemyAIType.BasicTracking;
    public float MoveSpeed = 3f;         // 기본 이동 속도
    public float DetectionRange = 10f;   // 플레이어 감지 범위 (그리드 단위)

    private bool _isMoving = false;
    public LayerMask UnitLayer;          // 플레이어 및 적들이 속한 레이어 (타일 점유 확인)
    public LayerMask ObstacleLayer;      // 장애물 체크용

    private EnemyStats _enemyStats;
    private PlayerStats _playerStats;

    void Awake()
    {
        _enemyStats = GetComponent<EnemyStats>();
        GameManager.Instance.RegisterEnemy(_enemyStats);
    }
    void Start()
    {
        _playerStats = GameManager.Instance.PlayerTransform.GetComponent<PlayerStats>();
    }

    // 적 턴에 호출되는 루틴 dotween으로 바꾸려다 코루틴이 더 적합해서 그대로 사용
    public IEnumerator TakeTurn()
    {
        //행동력 체크하기
        if (_enemyStats.BuffManager.GetFinalActionPoints() < 1f)
            yield break;

        switch (AIType)
        {
            case EnemyAIType.BasicTracking:
                yield return StartCoroutine(HandleBasicTrackingTween());
                break;
            default:
                yield return new WaitForSeconds(0.5f);
                break;
        }
    }
    // DOTween + WaitForCompletion() 리팩토링
    private IEnumerator HandleBasicTrackingTween()
    {
        // 현재 그리드 셀 계산
        Vector3 currentCell = new Vector3(
            Mathf.Floor(transform.position.x),
            Mathf.Floor(transform.position.y),
            transform.position.z);

        Vector3 enemyCenter = currentCell; // (0,0) 오프셋

        // 플레이어 위치 계산
        Vector3 playerCell = new Vector3(
            Mathf.Floor(_playerStats.transform.position.x),
            Mathf.Floor(_playerStats.transform.position.y),
            _playerStats.transform.position.z);

        Vector3 playerCenter = playerCell;

        float distanceToPlayer = Vector2.Distance(
            new Vector2(enemyCenter.x, enemyCenter.y),
            new Vector2(playerCenter.x, playerCenter.y));

        // 인접 시 공격
        if (distanceToPlayer <= 1f)
        {
            _enemyStats.Attack(_playerStats);
            _enemyStats.BuffManager.ApplyBuff(-1f, 0);
            yield break;
        }

        // 추적 이동
        if (distanceToPlayer <= DetectionRange)
        {
            Vector3 delta = playerCenter - enemyCenter;
            float dirX = Mathf.Abs(delta.x) > 0.1f ? Mathf.Sign(delta.x) : 0f;
            float dirY = Mathf.Abs(delta.y) > 0.1f ? Mathf.Sign(delta.y) : 0f;

            Vector3 targetCell = currentCell + new Vector3(dirX, dirY, 0);
            Vector3 destination = targetCell; // (0,0) 오프셋

            // 점유 검사
            Collider2D hit = Physics2D.OverlapPoint(destination, UnitLayer);
            if (hit != null && hit.CompareTag("Player"))
            {
                _enemyStats.Attack(_playerStats);
                _enemyStats.BuffManager.ApplyBuff(-1f, 0);
                yield break;
            }

            // 이동
            _isMoving = true;
            float dist = Vector3.Distance(transform.position, destination);
            float duration = dist / MoveSpeed;

            yield return transform
                .DOMove(destination, duration)
                .SetEase(Ease.Linear)
                .WaitForCompletion();

            // 타일 정보 갱신 (임시)
            _enemyStats.OnMoveTile(
                new Vector2Int(
                    Mathf.RoundToInt(currentCell.x),
                    Mathf.RoundToInt(currentCell.y)),
                new Vector2Int(
                    Mathf.RoundToInt(destination.x),
                    Mathf.RoundToInt(destination.y)));

            _enemyStats.BuffManager.ApplyBuff(-1f, 0);
            _isMoving = false;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
    //아래 코드는 WaitForCompletion()가 이동과 완료 대기를 한번에 처리할것으로 보이므로 주석처리됨
    
    //// 목표 셀까지 부드럽게 이동하는 코루틴
    //private IEnumerator MoveToTarget(Vector3 targetCell, bool isDiagonal = false)
    //{
    //    _isMoving = true;
    //    Vector3 destination = targetCell + new Vector3(0, 0, 0);
    //    float effectiveSpeed = MoveSpeed;
    //    enemyStats.OnMoveTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)),
    //        new Vector2Int(Mathf.RoundToInt(destination.x), Mathf.RoundToInt(destination.y)));// 임시로 tile 이동시 tile정보 갱신

    //    while ((destination - transform.position).sqrMagnitude > 0.001f)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, destination, effectiveSpeed * Time.deltaTime);
    //        yield return null;
    //    }
    //    transform.position = destination;
    //    _isMoving = false;
    //    yield break;
    //}
}
