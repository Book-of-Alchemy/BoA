using UnityEngine;
using System.Collections;

public enum EnemyAIType
{
    BasicTracking,
    // 확장 예정: Ranged, Patrol 등 추가 가능
}

public class EnemyController : MonoBehaviour
{
    public EnemyAIType AIType = EnemyAIType.BasicTracking;
    public float MoveSpeed = 3f;      // 기본 이동 속도 (카드널 기준)
    public float DetectionRange = 10f; // 플레이어 인식 범위 (그리드 단위)

    private bool _isMoving = false;
    private PlayerStats _playerStats;

    void Start()
    {
        // TurnManager의 플레이어 또는 씬 내의 PlayerStats 검색
        if (TurnManager.Instance != null && TurnManager.Instance.Player != null)
            _playerStats = TurnManager.Instance.Player;
        else
            _playerStats = FindObjectOfType<PlayerStats>();
    }

    // TurnManager에서 호출하는 적 턴 루틴
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

    // 기본 추적형 AI: 플레이어가 인식 범위 내이면 한 칸 단위 직선으로 이동
    private IEnumerator HandleBasicTracking()
    {
        // 현재 위치의 바닥 좌표 계산 (바둑판 셀의 좌측 하단 좌표)
        Vector3 currentCell = new Vector3(Mathf.Floor(transform.position.x),
                                          Mathf.Floor(transform.position.y),
                                          transform.position.z);
        // 현재 셀의 중앙 위치 구함
        Vector3 enemyCenter = currentCell + new Vector3(0.5f, 0.5f, 0);

        // 플레이어 위치도 동일하게 계산
        Vector3 playerCell = new Vector3(Mathf.Floor(_playerStats.transform.position.x),
                                         Mathf.Floor(_playerStats.transform.position.y),
                                         _playerStats.transform.position.z);
        Vector3 playerCenter = playerCell + new Vector3(0.5f, 0.5f, 0);

        float distance = Vector2.Distance(new Vector2(enemyCenter.x, enemyCenter.y),
                                          new Vector2(playerCenter.x, playerCenter.y));

        if (distance <= DetectionRange)
        {
            Vector3 delta = playerCenter - enemyCenter;
            float dirX = Mathf.Abs(delta.x) > 0.1f ? Mathf.Sign(delta.x) : 0f;
            float dirY = Mathf.Abs(delta.y) > 0.1f ? Mathf.Sign(delta.y) : 0f;

            bool isDiagonal = (dirX != 0f && dirY != 0f);

            // 목표 셀의 바닥 좌표: 현재 셀에서 (dirX, dirY)만큼 이동
            Vector3 targetCell = currentCell + new Vector3(dirX, dirY, 0);
            // MoveToTarget()에서는 바닥 좌표에 (0.5, 0.5)를 더해 센터를 구함
            yield return StartCoroutine(MoveToTarget(targetCell, isDiagonal));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }

    // 대상 셀까지 부드럽게 이동하는 코루틴  
    // targetCell은 바닥 좌표이므로, 목표 센터는 targetCell + (0.5, 0.5)
    private IEnumerator MoveToTarget(Vector3 targetCell, bool isDiagonal = false)
    {
        _isMoving = true;
        Vector3 destination = targetCell + new Vector3(0.5f, 0.5f, 0);
        float effectiveSpeed = MoveSpeed; // 필요시 isDiagonal에 따라 조정 가능

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
