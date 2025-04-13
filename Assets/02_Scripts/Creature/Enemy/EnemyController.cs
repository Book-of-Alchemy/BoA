using UnityEngine;
using System.Collections;

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

    // 플레이어 참조 (예: PlayerStats 등)
    private PlayerStats _playerStats;

    void Start()
    {
        // TurnManager나 다른 방법으로 플레이어 참조를 가져오거나,
        // 씬 내에서 PlayerStats 컴포넌트를 찾습니다.
        _playerStats = FindObjectOfType<PlayerStats>();
    }

    // 적 턴에 호출되는 루틴
    public IEnumerator TakeTurn()
    {
        switch (AIType)
        {
            case EnemyAIType.BasicTracking:
                yield return StartCoroutine(HandleBasicTracking());
                break;
            default:
                yield return new WaitForSeconds(0.5f);
                break;
        }
        yield break;
    }

    // 기본 추적형 AI: 플레이어가 감지 범위 내에 있으면 한 칸 단위로 이동
    private IEnumerator HandleBasicTracking()
    {
        // 현재 위치의 그리드 셀(바닥 좌표) 계산
        Vector3 currentCell = new Vector3(Mathf.Floor(transform.position.x),
                                          Mathf.Floor(transform.position.y),
                                          transform.position.z);
        // 현재 셀의 중앙 좌표
        Vector3 enemyCenter = currentCell + new Vector3(0.5f, 0.5f, 0);

        // 플레이어의 그리드 셀 및 중앙 좌표 계산
        Vector3 playerCell = new Vector3(Mathf.Floor(_playerStats.transform.position.x),
                                         Mathf.Floor(_playerStats.transform.position.y),
                                         _playerStats.transform.position.z);
        Vector3 playerCenter = playerCell + new Vector3(0.5f, 0.5f, 0);

        // 플레이어와 적의 거리 계산
        float distance = Vector2.Distance(new Vector2(enemyCenter.x, enemyCenter.y),
                                          new Vector2(playerCenter.x, playerCenter.y));

        if (distance <= DetectionRange)
        {
            // 목표 방향 결정
            Vector3 delta = playerCenter - enemyCenter;
            float dirX = Mathf.Abs(delta.x) > 0.1f ? Mathf.Sign(delta.x) : 0f;
            float dirY = Mathf.Abs(delta.y) > 0.1f ? Mathf.Sign(delta.y) : 0f;
            bool isDiagonal = (dirX != 0f && dirY != 0f);

            // 목표 셀 계산: 현재 셀 + (dirX, dirY)
            Vector3 targetCell = currentCell + new Vector3(dirX, dirY, 0);
            Vector3 targetPosition = targetCell + new Vector3(0.5f, 0.5f, 0);

            // 이동 시작 전에, 플레이어(또는 다른 유닛)가 해당 타일에 있는지 검사
            Collider2D hit = Physics2D.OverlapPoint(targetPosition, UnitLayer);
            if (hit != null && hit.gameObject.CompareTag("Player"))
            {
                Debug.Log("적 이동 검사: 목표 칸에 플레이어가 있어 이동하지 않습니다.");
                yield break; // 플레이어가 있다면 이동 취소
            }
            // 또는, UnitLayer에 속한 다른 유닛도 검사할 수 있음 (자기 자신은 제외)
            if (hit != null && hit.gameObject != gameObject)
            {
                Debug.Log("적 이동 검사: 목표 칸이 다른 유닛에 의해 점유되어 이동하지 않습니다.");
                yield break;
            }

            // 점유 검사를 통과하면, 해당 칸으로 이동
            yield return StartCoroutine(MoveToTarget(targetCell, isDiagonal));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }

    // 목표 셀까지 부드럽게 이동하는 코루틴
    // targetCell은 바닥 좌표이므로, 실제 목적지는 (targetCell + (0.5, 0.5))
    private IEnumerator MoveToTarget(Vector3 targetCell, bool isDiagonal = false)
    {
        _isMoving = true;
        Vector3 destination = targetCell + new Vector3(0.5f, 0.5f, 0);
        float effectiveSpeed = MoveSpeed;

        while ((destination - transform.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, effectiveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destination;
        _isMoving = false;
        yield break;
    }
}
